using System;

namespace Wikithis
{
	public partial class Wikithis
	{
		internal static bool CheckURLValid(string s) => Uri.TryCreate(s, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
	}
}
