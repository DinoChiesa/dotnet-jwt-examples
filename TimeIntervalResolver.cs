// Copyright 2018 Google LLC.
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
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Google.Apigee
{
  public static class TimeResolver {

    private const String expiryPattern = @"^([1-9][0-9]*)(ms|s|m|h|d|w|)$";

    private static Int64 TimeMultiplier(String s)
    {
      switch (s)
      {
        case "ms": return 1;
        case "s": return 1*1000;
        case "m": return 60*1000;
        case "h": return 60*60*1000;
        case "d": return 24*60*60*1000;
        case "w": return 7*24*60*60*1000;
      }
      return -1L;
    }

    private const String defaultUnit = "s";

    /*
     * convert a simple timespan string, expressed in days, hours, minutes, or
     * seconds, such as 30d, 12d, 8h, 24h, 45m, 30s, into a numeric quantity in
     * milliseconds. Default TimeUnit is ms. Eg. 30 is treated as 30ms.
     */
    public static Int64 ResolveExpression(String subject) {
      Match m = Regex.Match(subject, expiryPattern, RegexOptions.IgnoreCase);
      if (m.Success) {
        String key = m.Groups[2].Value;
        if(key.Equals(""))
          key = defaultUnit;
        return Int32.Parse(m.Groups[1].Value) * TimeMultiplier(key);
      }
      return -1L;
    }

  }

}