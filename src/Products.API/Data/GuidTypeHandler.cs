using System.Data;
using Dapper;

namespace Products.API.Data;

// SQLite guarda los Guid como TEXT, asi que Dapper no los castea solo al leer.
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value) => Guid.Parse((string)value);

    public override void SetValue(IDbDataParameter parameter, Guid value) =>
        parameter.Value = value.ToString();
}
