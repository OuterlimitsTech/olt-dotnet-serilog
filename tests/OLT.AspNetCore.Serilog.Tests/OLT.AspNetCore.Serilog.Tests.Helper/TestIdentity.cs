using OLT.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;

namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public class TestIdentity : OltIdentity
    {

        public override System.Security.Claims.ClaimsPrincipal Identity
        {
            get
            {
                var roles = new List<string>();
                return new GenericPrincipal(new GenericIdentity(nameof(TestIdentity)), roles.ToArray());
            }
        }

        public override string Username => nameof(TestIdentity);
        public override string NameId => $"{nameof(TestIdentity)}@unittest.com";
        public override string Email => $"{nameof(TestIdentity)}@unittesting.com";

        public override bool HasRole(string? claimName)
        {
            return true;
        }

        public override bool HasRole<TRoleEnum>(params TRoleEnum[] roles)
        {
            return roles?.Any(role => HasRole(role.GetCodeEnum())) == true;
        }
    }
}
