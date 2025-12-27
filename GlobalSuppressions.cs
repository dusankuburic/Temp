using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method naming convention uses underscores for readability")]
[assembly: SuppressMessage("Style", "CS8618:Non-nullable property must contain a non-null value when exiting constructor", Justification = "DTOs are initialized by frameworks (AutoMapper, EF Core, Model Binding)")]