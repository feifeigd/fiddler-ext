using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

public static class HandleBase
{
	[SuppressUnmanagedCodeSecurity]
	[SecurityCritical]
	[DllImport("wininet.dll", CharSet = CharSet.Unicode, EntryPoint = "InternetGetCookieExW", ExactSpelling = true, SetLastError = true)]
	internal static extern bool InternetGetCookieEx([In] string url, [In] string cookieName, [Out] StringBuilder cookieData, [In] [Out] ref uint pchCookieData, uint flags, IntPtr reserved);
		
	[SecurityCritical]
	public static string GetCookieInternal(string url, bool throwIfNoCookie)
	{
		uint num = 0u;
		uint flags = 8192u;
		if (HandleBase.InternetGetCookieEx(url, null, null, ref num, flags, IntPtr.Zero))
		{
			num += 1u;
			StringBuilder stringBuilder = new StringBuilder((int)num);
			if (HandleBase.InternetGetCookieEx(url, null, stringBuilder, ref num, flags, IntPtr.Zero))
			{
				new WebPermission(NetworkAccess.Connect, url).Demand();
				return stringBuilder.ToString();
			}
		}
		int lastWin32Error = Marshal.GetLastWin32Error();
		if (throwIfNoCookie || lastWin32Error != 259)
		{
			throw new Win32Exception(lastWin32Error);
		}
		return null;
	}

	public static void UIThreadInvoke(this Form form, MethodInvoker code)
	{
		if (form.InvokeRequired)
		{
			form.Invoke(code);
			return;
		}
		code();
	}

	public static long GetTimeStamp()
	{
		DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
		return (DateTime.Now.Ticks - dateTime.Ticks) / 10000L;
	}

	public static DateTime GetDateTimeFrom1970Ticks(long curSeconds)
	{
		return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds((double)curSeconds);
	}

	public static string FixedLength(string str, int length = 7)
	{
		string text = str;
		for (int i = str.Length; i < length; i++)
		{
			text = "0" + text;
		}
		return text;
	}

	public static string ToJson(this object obj)
	{
		return JsonConvert.SerializeObject(obj);
	}

	public static T ToObject<T>(this string json)
	{
		return JsonConvert.DeserializeObject<T>(json);
	}
}
