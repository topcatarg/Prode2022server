using Prode2022Server.Models;

namespace Prode2022Server.Helpers;

public class SettingHelpers
{
    public JWTSettings JwtSettings { get; set; } = new();
    
    public SettingHelpers(IConfiguration config)
    {
        config.GetSection("JWTSettings").Bind(JwtSettings);
    }
}
