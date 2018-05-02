// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Text;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Google.Apigee
{
    class JwtGenerator
    {
      string _name;
      long _expiry = 0; // seconds duration
      string _passphrase;
      const int DEFAULT_expiry = 1800;
      const string DEFAULT_passphrase = "Secret1234567890"; // must be at least 256 bits

      public static void Main(string[] args)
      {
        try
        {
          new JwtGenerator(args).Run();
        }
        catch (System.Exception exc1)
        {
          Console.WriteLine("Exception: {0}", exc1.ToString());
          Usage();
        }
      }

      long ConvertExpiryToSeconds(string spec)
      {
        return TimeResolver.ResolveExpression(spec)/1000;
      }
        
      public static void Usage()
      {
        Console.WriteLine("\nJwtGenerator: generate an Hmac256-signed JWT.\n");
        Console.WriteLine("Usage:\n  JwtGenerator --name <value> [--passphrase <key>] [--expiry <timevalue>]");
      }

      public JwtGenerator (string[] args)
      {
        for (int i=1; i < args.Length; i++)
        {
          switch (args[i])
          {
            case "--expiry":
              i++;
              if (args.Length <= i) throw new ArgumentException(args[i]);
              _expiry = ConvertExpiryToSeconds(args[i]);
              break;

            case "--name":
              i++;
              if (args.Length <= i) throw new ArgumentException(args[i]);
              _name = args[i];
              break;

            case "--passphrase":
              i++;
              if (args.Length <= i) throw new ArgumentException(args[i]);
              _passphrase = args[i];
              break;

            case "-?":
              throw new ArgumentException(args[i]);

            default:
              throw new ArgumentException(args[i]);
          }
        }

        if (_expiry == 0) {
          _expiry = DEFAULT_expiry;
        }
        if (_passphrase == null) {
          _passphrase = DEFAULT_passphrase;
          Console.WriteLine("using default passphrase of: {0}", _passphrase);
        }

        if (_name == null)
          throw new ArgumentException("--name");
      }

      int RenderAsEpoch(DateTime d)
      {
        TimeSpan span = d.Subtract(new DateTime(1970,1,1,0,0,0));
        return (int) span.TotalSeconds;
      }

      void Run()
      {
        var now = DateTime.UtcNow;
        var securityKey = new Microsoft.IdentityModel.Tokens.
          SymmetricSecurityKey(Encoding.UTF8.GetBytes(_passphrase));
        
        var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
        var header = new JwtHeader(signingCredentials);
        var payload = new JwtPayload
        {
          { "unique_name", _name },
          { "scope", "https://apigee.com/example"},
          { "iat", RenderAsEpoch(now)},
          { "nbf", RenderAsEpoch(now)},
          { "exp", RenderAsEpoch(now.AddSeconds(_expiry))}
        };
        var secToken = new JwtSecurityToken(header, payload);
        var handler = new JwtSecurityTokenHandler();
        var tokenString = handler.WriteToken(secToken);
        Console.WriteLine("\ntoken:\n" + tokenString);
        
        var decodedToken = handler.ReadToken(tokenString);
        Console.WriteLine("\nDecoded: \n"+ decodedToken);
      }
    }
}
