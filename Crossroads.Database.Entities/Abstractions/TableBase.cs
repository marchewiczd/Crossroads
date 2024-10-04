using System.ComponentModel.DataAnnotations;

namespace Crossroads.Database.Entities.Abstractions;

public abstract record TableBase
{
    public virtual object? GetKey() =>
        GetType().GetProperties()?
            .Select(pi => new { Property = pi, Attribute = pi.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() as KeyAttribute })?
            .FirstOrDefault(x => x.Attribute != null)?
            .Property.GetValue(this, null);
}