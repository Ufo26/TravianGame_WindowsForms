
using System;
using System.Drawing;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;

namespace UFO {
	static class Convert {
		/// <summary> ���������� ������ � ������� ���������� ������ ��� �����-���� ��������������. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> ���������� ������ �������� ���������� ������. </returns>
		public static Size ScreenBounds_Size() { return Screen.PrimaryScreen.Bounds.Size; }
		/// <summary> ���������� ������������� � ������� ���������� ������ ��� �����-���� ��������������. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> ���������� ������������� �������� ���������� ������. </returns>
		public static Rectangle Screen_Bounds() { return Screen.PrimaryScreen.Bounds; }
		/// <summary> ���������� ������ � ������� ���������� ������ ��� �����-���� ��������������. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> ���������� ������� ���������� ������. </returns>
		public static Size ScreenWorkingArea_Size() { return Screen.PrimaryScreen.WorkingArea.Size;	}
		/// <summary> ���������� ������������� � ������� ���������� ������ ��� �����-���� ��������������. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> ���������� ������� ���������� ������. </returns>
		public static Rectangle Screen_WorkingArea() { return Screen.PrimaryScreen.WorkingArea; }
		/// <summary>
		/// <b>C</b>urrent <b>S</b>creen <b>R</b>esolution - ������� ���������� ������. <br/>
		/// ������� ��������������� �������� <b> NUM </b> ���������� � ���������� ������ 1920�1080 � ������ ���������� ������. <br/>
		/// ������� ���� ������������ ����� ������� ��������. �������: <b>NUM / 1080.0 * Screen...Height;</b>
		/// </summary>
		/// <returns> ���������� ��������������� <b> NUM </b> � ������� ���������� ������. </returns>
		public static int ToCSR(int NUM) { return (int)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static float ToCSR(float NUM) { return (float)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height);	}
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static double ToCSR(double NUM) { return (double)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }

		/// <summary> 
		///		������� ��������������� ���������� ����� � ������� (� ������ 86'400 ������ = [0..86'399]). <br/>
		///		��������� - ��� ����. �������� ���� � ��������� ����� 48.��� - ��� ����� 48 �����, 2 ������. <br/>
		///		�������� - ��� �������� �� ����. ������� 0.1428 = 60/100 * 14,28% = 8,568 = 8 ����� + 56,8% ����� = 60/100 * 56,8 = 34 ������ + 0,8% �������.
		/// </summary>
		/// <returns> ���������� ����� � ��������. <br/>
		///		���� �������� <b> Double </b> �������� 24.��� ����, �� ������� ����� ����� ����������� 86'399.
		/// </returns>
		public static int ToTimeSecond(double Double) { return (int)(Double * 3600); }

		/// <summary> 
		///		������� ��������������� ���������� ����� � ����������� (� ������ 86'400'000 ����������� = [0..86'399'999]). <br/>
		///		��������� - ��� ����. �������� ���� � ��������� ����� 48.��� - ��� ����� 48 �����, 2 ������. <br/>
		///		�������� - ��� �������� �� ����. ������� 0.1428 = 60/100 * 14,28% = 8,568 = 8 ����� + 56,8% ����� = 60/100 * 56,8 = 34 ������ + 0,8% ������� = 1000/100 * 8 = 80 �������.
		/// </summary>
		/// <returns> ���������� ����� � ������������. <br/>
		///		���� �������� <b> Double </b> �������� 24.��� ����, �� ������� ����� ����� ����������� 86'399'999.
		/// </returns>
		public static int ToTimeMilliSecond(double Double) { return (int)(Double * 3600000); }

		/// <summary> ������� ��������������� ���������� ������� � ������� (� ������ 86'400 ������ = [0..86'399]). </summary>
		/// <remarks> <b> hour: </b> ����. <br/> <b> min: </b> ������. <br/> <b> sec: </b> �������. <br/> </remarks>
		/// <returns> ���������� ����� � ��������. <br/>
		///		���� �������� <b> hour </b> �������� 24 ����, �� ������� ����� ����� ����������� 86'399.
		/// </returns>
		public static uint ToTimeSecond(uint hour, uint min, uint sec) {
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> ������� ��������������� ���������� ������� � ������������ (� ������ 86'400'000 ����������� = [0..86'399'999]). </summary>
		/// <remarks> <b> hour: </b> ����. <br/> <b> min: </b> ������. <br/> <b> sec: </b> �������. <br/> <b> msec: </b> ������������. <br/> </remarks>
		/// <returns> ���������� ����� � �������������. <br/>
		///		���� �������� <b> hour </b> �������� 24 ����, �� ������� ����� ����� ����������� 86'399'999.
		/// </returns>
		public static uint ToTimeMilliSecond(uint hour, uint min, uint sec, uint msec) {
			return hour * 3600000 + min * 60000 + sec * 1000 + msec;
		}

		/// <summary> ������� ��������������� ����� ["��:��:��"] � �������. </summary>
		/// <remarks> <b> time: </b> ������ ������� <b> string </b> ������� ["��:��:��"]. <br/> </remarks>
		/// <returns> ���������� ����� � �������������. <br/>
		///		���� �������� <b> time </b> �������� 24 ����, �� ������� ����� ����� ����������� 86'399'999.
		/// </returns>
		public static int ToTimeSecond(string time) {
			int x = 0, hour = 0, min = 0, sec = 0; string tmp = ""; time += ":";
            for (int i = 0; i < time.Length; i++) if (time[i] != ':') tmp += time[i]; else {
				try { 
					if (x == 0) hour = System.Convert.ToInt32(tmp);
					if (x == 1) min = System.Convert.ToInt32(tmp);
					if (x == 2) sec = System.Convert.ToInt32(tmp);
				}
				catch (FormatException) {
					System.Windows.MessageBox.Show("������. class Convert;\n������ time = '" + time + "' ����� �� ������ ������.\n" +
						"���������� ������ ��� ���������: '��:��:��'. ������: time = '12:36:28'.\n return 0;"
					);
					return 0;
                }
				x++; tmp = "";
            }
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> ������� ��������������� ������� � ������ ["��:��:��"] (� ������ 86'400 ������ = [0..86'399]). </summary>
		/// <remarks> 
		///		<b> second: </b> ����� � ��������. <br/>
		///		<b> h: </b> ��������� ����������� "�" - ���� �� ��������� �����. <br/>
		///		<b> m: </b> ��������� ����������� "�" - ������ �� ��������� �����. <br/>
		///		<b> s: </b> ��������� ����������� "�" - ������� �� ��������� �����. <br/>
		/// </remarks>
		/// <returns> ���������� ����� ��� ������ <b>["��:��:��"]</b>. � ������������� �� ��������� �����, <br/>
		///		������: <b>03�:12�:47�</b>. ���� ����������� �� ���������, ������ ����� ����� ���: <b>03:12:47</b> <br/>
		///		���� �������� <b>second</b> �������� <b>86'399</b>, �� ������� ����� ����� ����������� <b>"23:59:59"</b>. <br/>
		///		���� �������� <b>second</b> ����� �������������, �� ������� �����: <b>"?:?:?"</b>
		/// </returns>
		public static string ToTimeString(int second, string h = "", string m = "", string s = "") {
			if (second < 0) return "?:?:?";	string time = "";
			int hour = second / 3600; if (hour < 10) time = "0";
			time += (hour + h + ":");
			int min = second / 60 % 60; if (min < 10) time += "0";
			time += (min + m + ":");
			int sec = second % 60; if (sec < 10) time += "0";
			if (sec < 0) time += "?"; else time += (sec + s);
			return time;
		}
		/// <summary> ������� ��������������� ������������ � ������ ["��:��:��:��"] (� ������ 86'400'000 ����������� = [0..86'399'999]). </summary>
		/// <remarks> <b> millisecond: </b> ����� � �������������. <br/> </remarks>
		/// <returns> ���������� ����� ��� ������ ["��:��:��:��"]. <br/>
		///		���� �������� <b> millisecond </b> �������� 86'399'999, �� ������� ����� ����� ����������� "23:59:59:999". <br/>
		///		���� �������� <b>millisecond</b> ����� �������������, �� ������� �����: <b>"??:??:??:???"</b>
		/// </returns>
		public static string MStoTime(uint millisecond) {
			if (millisecond < 0) return "??:??:??:???";	string time = "";
			uint hour = millisecond / 3600000; if (hour < 10) time = "0";
			time += hour + ":";
			uint min = millisecond / 60000 % 60000; if (min < 10) time += "0";
			time += min + ":";
			uint sec = millisecond / 1000 % 60;	if (sec < 10) time += "0";
			time += sec + ":";
			uint msec = millisecond % 1000;	if (msec < 10) time += "00"; else if (msec < 100) time += "0";
			time += msec;
			return time;
		}

		/// <summary> ����� ����������� ����� <b>value</b> �������� ������� ����� ���������. ������: 1'000'000'000. </summary>
		/// <value>	
		///		<b><paramref name="value"/>:</b> ����� � ������� string. <br/>
		///		<b><paramref name="tab"/>:</b> �����������. �� ��������� ������ ����������� ���������. <br/>
		/// </value>
		/// <returns> ���������� ����������������� ����� � ���� ������ � ��������� ����� ���������: xx'xxx'xxx'xxx. </returns>
		public static string toTABString(string value, string tab = " ") {
			if (value.Length < 4) return value; else {
				char[] chr = new char[value.Length + (value.Length - 1) / 3];	int x = chr.Length; int counter = 0;
                for (int i = value.Length - 1; i >= 0; i--) {
					x--; counter++;	if (counter == 4) { chr[x] += tab[0]; counter = 0; i++; } else chr[x] += value[i];
				} return new string(chr);
			}
		}

		/// <summary> �������� ���� ������������� ���������� ������. <br/> ����� ���������: ������ �� ������� ������ ������ � �������� ��������. <br/>	��� ���������� ������ �� ��������, ����� �������� ������ ������ ��� ������� �������. </summary>
		/// <value>	<b><paramref name="min"/>:</b> ����������� ������ ������. <br/>	<b><paramref name="max"/>:</b> ������������ ������ ������. </value>
		/// <returns> ���������� <b>true</b> ���� ������ ������ �������� ���������� �������� � ���������. <br/> ���������� <b>false</b> ���� ��� ��������� ��������� <b>default</b> ��� �������� ��� ���������. </returns>
		public static bool SCREEN(int min = -1, int max = -1) {
			if (min <= -1 && max <= -1) return false;
			else {
				if (max <= -1) { if (Screen.PrimaryScreen.Bounds.Size.Height < min) return true; } 
					else if (min <= -1) { if (Screen.PrimaryScreen.Bounds.Size.Height >= max) return true; }
						else if (Screen.PrimaryScreen.Bounds.Size.Height < max &&
								 Screen.PrimaryScreen.Bounds.Size.Height >= min) return true;
						return false;
			}
		}

		/// <summary> ����� ����������� ���������������� ����� � ���������� ����� <b>RGB</b>. <br/> ����� ��������� ������ � ������� ������� ���������������� �����. </summary>
		/// <returns> ���������� ���������� ����� <b>RGB</b> � ��������� [0..255]. </returns>
		public static byte ToRGB<T> (T cl) {
			byte result = 0; ulong unsigned_cl = 0; long signed_cl = -1;
			if (cl is ulong) unsigned_cl = (ulong)(object)cl;	if (cl is long) signed_cl = (long)(object)cl;
			if (cl is uint) unsigned_cl = (uint)(object)cl;		if (cl is int) signed_cl = (int)(object)cl;
			if (cl is ushort) unsigned_cl = (ushort)(object)cl;	if (cl is short) signed_cl = (short)(object)cl;
			if (cl is sbyte) unsigned_cl = (byte)(object)cl;	if (cl is byte) signed_cl = (byte)(object)cl;
			if (cl is ulong || cl is uint || cl is ushort || cl is sbyte)
				if (unsigned_cl <= 0) result = 0; else if (unsigned_cl >= 255) result = 255; else result = (byte)unsigned_cl;
			if (typeof(T) == typeof(long) || typeof(T) == typeof(int) || typeof(T) == typeof(short) || typeof(T) == typeof(byte)) 
				if (signed_cl <= 0) result = 0; else if (signed_cl >= 255) result = 255; else result = (byte)signed_cl;
			return result;
		}

		/// <summary> ����� ��������� ������/������� ������� ���������� ����� � ������������ � ������ �����������. </summary>
		/// <remarks> ��������� �������� <b>cl</b> �������� ���������� � ���� ���������� ����. </remarks>
		public static void toRGB(ref int cl) { if (cl < 0) cl = byte.MinValue; else if (cl > 255) cl = byte.MaxValue; }
		/// <summary> ����� ��������� ������/������� ������� ������ ���������� ����� � ������������ � ������ �����������. </summary>
		/// <remarks> ��������� �������� �������� ����������� � ���� ���������� ����. </remarks>
		public static void toRGB(ref int R, ref int G, ref int B, ref int A) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;	if (A < 0) A = 0; else if (A > 255) A = 255;
		}
		/// <summary> ����� ��������� ������/������� ������� ������ ���������� ����� � ������������ � ������ �����������. </summary>
		/// <remarks> ��������� �������� �������� ����������� � ���� ���������� ����. </remarks>
		public static void toRGB (ref int R, ref int G, ref int B) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;
		}
		/// <summary> ����� ��������� ����� ������������� �������� �� ���� ����� � ������ <b>Alpha</b>. <br/> ��� ���������� ����� ���� ��� ���������� �����, ��� � ������ ��������, ��� ����� ������������. <br/> �������� <paramref name="Alpha"/> ����������� �� ���������� ��������, � ������ ������ �� ���������, ���������� � ������ ��� ������� �������. </summary>
		/// <value>
		///		<b><paramref name="First"/>:</b> ������ ��������, ��� �������� ����������� ������������ <b>Alpha</b>. <br/> <b><paramref name="Second"/>:</b> ������ ��������, �� �������� ���������� ������������. <br/>
		///		<b><paramref name="Alpha"/>:</b> �������� ������������ ��� ������������ ��� �������� <paramref name="First"/>, ��������: <b>[0..1],</b> <br/> ��� 0 - ��������� ����������, 1 - 100% �� ����������, 0.5 - �������������� (���� ��������� �������� ������������ �����). <br/>
		/// </value>
		/// <returns> ���������� ����� ������������� �������� � ������ <b>Alpha</b>. </returns>
		public static double Interpolate(double First, double Second, double Alpha) {
			Alpha = Alpha < 0 ? 0 : Alpha > 1 ? 1 : Alpha;//[0..1]
			//return First * (Alpha / 255) + Second * (1 - (Alpha / 255));
			return First * Alpha + Second * (1.0 - Alpha);
		}

		/// <summary> ����� ������������ �������� ������ <b>RGB</b> (Color) � �������� ������ <b>HSV.</b> </summary>
		/// <returns> ���������� ��������������� �������� ������ <b>HSV.</b> </returns>
		public static HSV ColorToHSV(this Color color) {
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));
			double hue = color.GetHue();
			double saturation = (max == 0) ? 0 : 1.0 - (1.0 * min / max);
			double value = max / 255.0;
			return new HSV(hue, saturation, value);
		}
		/// <summary> ����� ������������ �������� ������ <b>HSV</b> � �������� ������ <b>RGB</b> (Color). </summary>
		/// <returns> ���������� ��������������� �������� ������ <b>RGB</b> (Color). </returns>
		public static Color HSVToColor(this HSV hsv) {
			int hi = System.Convert.ToInt32(Math.Floor(hsv.Hue / 60)) % 6;
			double f = hsv.Hue / 60 - Math.Floor(hsv.Hue / 60);
			double value = hsv.Value * 255;
			int v = System.Convert.ToInt32(value);
			int p = System.Convert.ToInt32(value * (1 - hsv.Saturation));
			int q = System.Convert.ToInt32(value * (1 - f * hsv.Saturation));
			int t = System.Convert.ToInt32(value * (1 - (1 - f) * hsv.Saturation));
			return (hi == 0) ? Color.FromArgb(255, v, t, p) : (hi == 1) ? Color.FromArgb(255, q, v, p) :
				(hi == 2) ? Color.FromArgb(255, p, v, t) : (hi == 3) ? Color.FromArgb(255, p, q, v) :
					(hi == 4) ? Color.FromArgb(255, t, p, v) : Color.FromArgb(255, v, p, q);
		}
	}
}