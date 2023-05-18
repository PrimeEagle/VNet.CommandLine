using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VNet.CommandLine.Validation;
using VNet.CommandLine.Validation.FluentValidation;
using VNet.Utility.Extensions;
using VNet.Validation;

namespace VNet.CommandLine
{
	public class DefaultParser : IParser
	{
		// ReSharper disable once UnusedMember.Global
		public IUsage Parse(string[] args, IConfiguration configuration,
								Validation.ICommandLineValidatorManager validator)
		{
			var normalizedArgs = NormalizeArguments(args);
			var result = ParseArguments(normalizedArgs, configuration, validator);
			result = InstantiateCategories(result);

			return result;
		}

		private static string[] NormalizeArguments(string[] args)
		{
			var joinedArgs = string.Join(' ', args).Replace(", ", ",").Replace("; ", ";");

			var options = RegexOptions.None;
			var regex = new Regex("[ ]{2,}", options);
			joinedArgs = regex.Replace(joinedArgs, " ");

			foreach (var s in DefaultValues.OptionValueSeparators)
			{
				joinedArgs = joinedArgs.Replace($"{s} ", $"{s}");
			}

			var splitArgs = joinedArgs.Split(' ');

			return splitArgs.Select(x => x.Replace(", ", ",")
												.Replace("; ", ";")
												.Replace(" ,", ",")
												.Replace(" ;", ";")
												.ToLower()
												.Trim()
							).ToArray();
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		private static IUsage ParseArguments(string[] args, IConfiguration configuration,
			ICommandLineValidatorManager validator)
		{
			var result = new Usage();
			var skip = false;

			for (var i = 0; i < args.Length; i++)
			{
				if (skip)
				{
					skip = false;
					continue; ;
				}

				var category = GetValidCategory(args[i], configuration);
				if (category is not null)
				{
					category.Usage = result;
					result.Categories.Add(category);

					continue;
				}

				var verb = GetValidVerb(args[i], configuration);
				if (verb is not null)
				{
					verb.Usage = result;
					result.Verbs.Add(verb);

					if (result.Categories.Any())
					{
						result.Categories.Last().Verbs.Add(verb);
					}

					continue;
				}

				var option = GetValidOption(args[i], configuration, validator, out var validity);
				result.Validity.Append(validity);

				var argStart = new ArgumentStart()
				{
					DefaultVerb = configuration.DefaultVerb,
					Verbs = result.Verbs,
					Option = option
				};

				var argStartValidator =
					(ArgumentStartValidator)validator.ValidatorParameters.AdditionalValidators[2];
				var argStartValidity = argStartValidator.DoValidate(argStart, new List<string> { "Usage" },
					ErrorCategory.Usage);
				result.Validity.Append(argStartValidity);

				if (option is not null)
				{
					if (validity.Valid && option.DataType is not null && !option.AllowNullValue &&
						option.NeedsValue &&
						option.DataValue is null && (i + 1) < args.Length)
					{
						option.DataValue = Convert.ChangeType(args[i + 1], option.DataType);
						skip = true;
					}

					if (result.Verbs.Any())
					{
						option.ArgumentVerb = result.Verbs.Last();
						result.Verbs.Last().ArgumentOptions.Add(option);
					}
					else if (configuration.DefaultVerb is not null)
					{
						option.ArgumentVerb = configuration.DefaultVerb;
						configuration.DefaultVerb.ArgumentOptions.Add(option);
					}
				}
			}
			return result;
		}

		private static ICategory GetValidCategory(string name, IConfiguration configuration)
		{
			var categoryNames = configuration.Categories.SelectMany(c => c.AllNames);

			foreach (var p in DefaultValues.CategoryPrefixes)
			{
				if (!name.StartsWith(p)) continue;

				var baseCategory = name[p.Length..];

				if (categoryNames.Contains(baseCategory))
				{
					var category = configuration.Categories
						.First(c => c.AllNames.Contains(baseCategory));

					return (ICategory)category.Clone();
				}
			}

			return null;
		}
		
		private static IVerb GetValidVerb(string name, IConfiguration configuration)
		{
			var verbNames = configuration.Categories.SelectMany(c => c.Verbs).SelectMany(v => v.AllNames);

			foreach (var p in DefaultValues.VerbPrefixes)
			{
				if (!name.StartsWith(p)) continue;

				var baseVerb = name[p.Length..];

				if (verbNames.Contains(baseVerb))
				{
					var verb = configuration.Categories.SelectMany(c => c.Verbs)
						.First(v => v.AllNames.Contains(baseVerb));

					return (IVerb)verb.Clone();
				}
			}

			return null;
		}

		private static IOption GetValidOption(string name, IConfiguration configuration, ICommandLineValidatorManager validator, out ValidationState validity)
		{
			validity = new ValidationState();

			foreach (var p in DefaultValues.OptionPrefixes)
			{
				if (!name.StartsWith(p)) continue;

				var baseOptionName = name[p.Length..];

				SplitOptionNameValue(baseOptionName, out var optionName, out var optionVal, validator, out var splitValidity);
				validity.Append(splitValidity);

				if (configuration.Categories.SelectMany(c => c.Options).SelectMany(o => o.AllNames).Contains(optionName))
				{
					var option = configuration.Categories.SelectMany(c => c.Options)
						.First(o => o.AllNames.Contains(optionName));

					if (option.DataType == typeof(Array))
					{
						var av = new ArgumentArrayValue()
						{
							ArrayString = optionVal
						};

						var avValidator = (ArgumentArrayValueValidator)validator.ValidatorParameters.AdditionalValidators[0];
						var avValidationState = avValidator.DoValidate(av, ErrorCategory.Usage);

						validity.Append(avValidationState);

						if (validity.Valid)
						{
							var delimiter =
								DefaultValues.OptionValueArrayDelimiters.First(d =>
									optionVal.Contains(d));

							option.DataValue = optionVal[1..^1]
								.Split(delimiter);
						}
					}
					else if (option.DataType.IsEnum)
					{
						if (Enum.TryParse(option.DataType, optionVal, true, out var enumResult))
						{
							option.DataValue = enumResult;
						}
					}
					else
					{
						try
						{
							option.DataValue = (optionVal is null)
								? null
								: Convert.ChangeType(optionVal, option.DataType);
						}
						catch (Exception e)
						{
							if (e is FormatException or InvalidCastException)
							{
								option.DataValue = null;
							}
						}

					}

					var newOption = (IOption)option.Clone();

					return newOption;
				}
			}

			return null;
		}

		private static void SplitOptionNameValue(string baseOptionName, out string optionName, out string optionVal,
			ICommandLineValidatorManager validator, out ValidationState validity)
		{
			validity = new ValidationState();

			optionName = baseOptionName;
			optionVal = null;

			var vp = new ArgumentValuePair()
			{
				ArgumentValue = baseOptionName
			};

			var vpValidator = (ArgumentValuePairValidator)validator.ValidatorParameters.AdditionalValidators[1];
			var validationState = vpValidator.DoValidate(vp, ErrorCategory.Usage);

			validity.Append(validationState);

			if (validationState.Valid)
			{
				var sep = DefaultValues.OptionValueSeparators.FirstOrDefault(baseOptionName.Contains);

				if (sep is not null)
				{
					optionName = baseOptionName.Before(sep);
					optionVal = baseOptionName.After(sep);
				}
			}
		}

		private IUsage InstantiateCategories(IUsage usage)
        {
			foreach (var verb in usage.Verbs)
			{
				foreach (var option in verb.ArgumentOptions)
				{
					var propInfo = option.Property;
					if(propInfo.DeclaringType is null) continue;
                    ;

                    var category = Activator.CreateInstance(propInfo.DeclaringType);
					if(!usage.InstantiatedCategories.ContainsKey(propInfo.DeclaringType)) 
                        usage.InstantiatedCategories.Add(propInfo.DeclaringType, category);

					if (option.DataType == typeof(Array))
					{
						var array = ((Array)option.DataValue);
						var elementType = option.DataValue?.GetType().GetElementType();
						if (elementType is not null)
						{
							var newArray = Array.CreateInstance(elementType, array.Length);
							Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
							array = newArray;
						}

						propInfo.SetValue(category, array);
					}
					else
                    {
                        propInfo.SetValue(category,
                            option.DataValue is null ? null : Convert.ChangeType(option.DataValue, option.DataType));
                    }
				}
			}

			return usage;
		}
	}
}