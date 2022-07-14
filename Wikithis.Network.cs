using System;
#if WINDOWS
using System.Net.Sockets;
using System.Net.NetworkInformation;
#endif
using System.Threading.Tasks;

namespace Wikithis
{
	public partial class Wikithis
	{
		internal static bool CheckURLValid(string s) => Uri.TryCreate(s, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

#pragma warning disable CS1998
		internal static async Task<bool> CheckNetworkConnection()
#pragma warning restore CS1998
		{
#if WINDOWS
			const int maxHops = 30;
			const string someFarAwayIpAddress = "8.8.8.8";

#pragma warning disable CS0162
			for (int ttl = 1; ttl <= maxHops; ttl++)
			{
				PingOptions options = new(ttl, true);
				byte[] buffer = new byte[32];
				PingReply reply;
				try
				{
					using Ping pinger = new();
					reply = await pinger.SendPingAsync(someFarAwayIpAddress, 10000, buffer, options);
				}
				catch
				{
					return false;
				}

				string address = reply.Address?.ToString() ?? null;
				if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.Success)
					return false;

				if (IsRoutableAddress(reply.Address))
			if (true)
				{
					return true;
				}
			}
			return false;
#pragma warning restore CS0162
#else
			return true;
#endif
		}

#if WINDOWS
		private static bool IsRoutableAddress(IPAddress addr)
		{
			if (addr == null)
			{
				return false;
			}
			else if (addr.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return !addr.IsIPv6LinkLocal && !addr.IsIPv6SiteLocal;
			}
			else // IPv4
			{
				byte[] bytes = addr.GetAddressBytes();
				if (bytes[0] == 10)
				{   // Class A network
					return false;
				}
				else if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
				{   // Class B network
					return false;
				}
				else if (bytes[0] == 192 && bytes[1] == 168)
				{   // Class C network
					return false;
				}
				else
				{   // None of the above, so must be routable
					return true;
				}
			}
		}
#endif
	}
}
