using System.Reflection;

namespace Tidy.AdventOfCode
{
    /// <summary>Responsible for creating <see cref="IDay"/> objects. This resolver uses the provided <see cref="IServiceProvider"/> for constructing the <see cref="IDay"/> instances by providing services to the <see cref="IDay"/>'s (single, public) constructor.</summary>
    public class DayResolver : IDayResolver
    {
        /// <summary>The constructor instances stored for each year-day pair.</summary>
        public IReadOnlyDictionary<(int year, int day), ConstructorInfo> DayConstructors { get; }

        /// <summary>Create a resolver object for constructing <see cref="Day{T}"/> instances using dependency injection.</summary>
        /// <param name="parameterValidator">The validator used to validate year and day values.</param>
        /// <param name="serviceProvider">The service provider used to provide required services for the <see cref="IDay"/> constructors.</param>
        /// <param name="additionalAssemblies">The assemblies to be scanned (in addition to the entry assembly) for <see cref="IDay"/> implementations.</param>
        public DayResolver(IParameterValidator parameterValidator, IServiceProvider serviceProvider, Assembly[]? additionalAssemblies)
        {
            ParameterValidator = parameterValidator;
            ServiceProvider = serviceProvider;

            var baseDayType = typeof(IDay);
            var constructors = new Dictionary<(int year, int day), ConstructorInfo>();
            foreach (var dayType in new[] { Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("You have to use Tidy Advent of Code from a managed entry assembly.") }.Concat(additionalAssemblies ?? Enumerable.Empty<Assembly>()).SelectMany(t => t.DefinedTypes).Where(t => t.IsClass && !t.IsAbstract && baseDayType.IsAssignableFrom(t)))
            {
                var attribute = dayType.GetCustomAttribute<DayAttribute>();
                int year, day;
                if (attribute != null)
                {
                    parameterValidator.Validate(attribute.Year, attribute.DayNumber);
                    if (attribute.Ignore)
                        continue;
                    else
                        (year, day) = (attribute.Year, attribute.DayNumber);
                }
                else
                {
                    var lastNamespacePart = (dayType.Namespace ?? "").Split('.')[^1];
                    if (!lastNamespacePart.StartsWith("Year") || !int.TryParse(lastNamespacePart[4..], out year) || !dayType.Name.StartsWith("Day") || !int.TryParse(dayType.Name[3..], out day))
                        throw new InvalidOperationException($"Every \"{nameof(IDay)}\" implementation, including \"{dayType.FullName}\" should either be annotated with the \"{nameof(DayAttribute)}\" attribute, or be placed in a namespace which is ending with the Year{{####}} segment, where #### is the full number of the corresponding Advent of Code year and also be named Day{{##}}, where ## is the one or two-digit number of the day for the calendar.");
                }
                if (dayType.GetConstructors().Length != 1)
                    throw new InvalidOperationException($"Every \"{nameof(IDay)}\" implementation, including \"{dayType.FullName}\" should have exactly one public constructor to be able to be constructed using the current resolver's service provider instance.");
                parameterValidator.Validate(year, day);
                if (constructors.TryGetValue((year, day), out var duplicateConstructor))
                    throw new InvalidOperationException($"Multiple types/constructors found for year {year}, day {day}: \n{duplicateConstructor.DeclaringType?.FullName}\n{dayType.FullName}");
                constructors.Add((year, day), dayType.GetConstructors()[0]);
            }
            DayConstructors = constructors;
        }

        /// <summary>The validator used to validate year and day values.</summary>
        public IParameterValidator ParameterValidator { get; }
        /// <summary>The service provider used to provide required services for the <see cref="Day{T}"/> constructors.</summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>Create a <see cref="IDay"/> implementation for a given <paramref name="year"/>-<paramref name="day"/> pair by using the registered implementation's single public constructor for dependency injection from the <see cref="ServiceProvider"/> instance.</summary>
        public IDay CreateDay(int year, int day)
        {
            ParameterValidator.Validate(year, day);
            if (!DayConstructors.TryGetValue((year, day), out var constructor))
                throw new InvalidOperationException($"There was no \"{nameof(IDay)}\" constructor found registered for the provided values (year {year}, day {day}).");

            return constructor.Invoke(constructor.GetParameters().Select(p => ServiceProvider.GetService(p.ParameterType)).ToArray()) as IDay ?? throw new InvalidCastException($"The registered constructor for the provided values (year {year}, day {day} returned an instance that is not assignable to \"{nameof(IDay)}\".");
        }
    }
}
