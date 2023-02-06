
using System;
using System.Drawing;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;

namespace UFO {
	static class Convert {
		/// <summary> Возвращает размер в текущем разрешении экрана без каких-либо преобразований. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> Возвращает размер текущего разрешения экрана. </returns>
		public static Size ScreenBounds_Size() { return Screen.PrimaryScreen.Bounds.Size; }
		/// <summary> Возвращает прямоугольник в текущем разрешении экрана без каких-либо преобразований. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> Возвращает прямоугольник текущего разрешения экрана. </returns>
		public static Rectangle Screen_Bounds() { return Screen.PrimaryScreen.Bounds; }
		/// <summary> Возвращает размер в текущем разрешении экрана без каких-либо преобразований. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> Возвращает текущее разрешение экрана. </returns>
		public static Size ScreenWorkingArea_Size() { return Screen.PrimaryScreen.WorkingArea.Size;	}
		/// <summary> Возвращает прямоугольник в текущем разрешении экрана без каких-либо преобразований. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> Возвращает текущее разрешение экрана. </returns>
		public static Rectangle Screen_WorkingArea() { return Screen.PrimaryScreen.WorkingArea; }
		/// <summary>
		/// <b>C</b>urrent <b>S</b>creen <b>R</b>esolution - Текущее Разрешение Экрана. <br/>
		/// Функция преобразовывает значение <b> NUM </b> записанное в разрешении экрана 1920х1080 в теущее разрешение экрана. <br/>
		/// Расчёты идут относительно всего дисплея монитора. Формула: <b>NUM / 1080.0 * Screen...Height;</b>
		/// </summary>
		/// <returns> Возвращает преобразованный <b> NUM </b> в текущем разрешении экрана. </returns>
		public static int ToCSR(int NUM) { return (int)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static float ToCSR(float NUM) { return (float)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height);	}
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static double ToCSR(double NUM) { return (double)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }

		/// <summary> 
		///		Функция преобразовывает десятичную дробь в секунды (в сутках 86'400 секунд = [0..86'399]). <br/>
		///		Числитель - это часы. Например если в числителе будет 48.ххх - это равно 48 часам, 2 суткам. <br/>
		///		Мантисса - это проценты по сути. Напрмер 0.1428 = 60/100 * 14,28% = 8,568 = 8 часов + 56,8% минут = 60/100 * 56,8 = 34 минуты + 0,8% милисек.
		/// </summary>
		/// <returns> Возвращает время в секундах. <br/>
		///		Если значение <b> Double </b> превысит 24.ххх часа, то функция вернёт число превышающее 86'399.
		/// </returns>
		public static int ToTimeSecond(double Double) { return (int)(Double * 3600); }

		/// <summary> 
		///		Функция преобразовывает десятичную дробь в милисекунды (в сутках 86'400'000 миллисекунд = [0..86'399'999]). <br/>
		///		Числитель - это часы. Например если в числителе будет 48.ххх - это равно 48 часам, 2 суткам. <br/>
		///		Мантисса - это проценты по сути. Напрмер 0.1428 = 60/100 * 14,28% = 8,568 = 8 часов + 56,8% минут = 60/100 * 56,8 = 34 минуты + 0,8% милисек = 1000/100 * 8 = 80 милисек.
		/// </summary>
		/// <returns> Возвращает время в милисекундах. <br/>
		///		Если значение <b> Double </b> превысит 24.ххх часа, то функция вернёт число превышающее 86'399'999.
		/// </returns>
		public static int ToTimeMilliSecond(double Double) { return (int)(Double * 3600000); }

		/// <summary> Функция преобразовывает компоненты времени в секунды (в сутках 86'400 секунд = [0..86'399]). </summary>
		/// <remarks> <b> hour: </b> часы. <br/> <b> min: </b> минуты. <br/> <b> sec: </b> секунды. <br/> </remarks>
		/// <returns> Возвращает время в секундах. <br/>
		///		Если значение <b> hour </b> превысит 24 часа, то функция вернёт число превышающее 86'399.
		/// </returns>
		public static uint ToTimeSecond(uint hour, uint min, uint sec) {
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> Функция преобразовывает компоненты времени в миллисекунды (в сутках 86'400'000 миллисекунд = [0..86'399'999]). </summary>
		/// <remarks> <b> hour: </b> часы. <br/> <b> min: </b> минуты. <br/> <b> sec: </b> секунды. <br/> <b> msec: </b> миллисекунды. <br/> </remarks>
		/// <returns> Возвращает время в миллисекундах. <br/>
		///		Если значение <b> hour </b> превысит 24 часа, то функция вернёт число превышающее 86'399'999.
		/// </returns>
		public static uint ToTimeMilliSecond(uint hour, uint min, uint sec, uint msec) {
			return hour * 3600000 + min * 60000 + sec * 1000 + msec;
		}

		/// <summary> Функция преобразовывает время ["чч:мм:сс"] в секунды. </summary>
		/// <remarks> <b> time: </b> строка времени <b> string </b> формата ["чч:мм:сс"]. <br/> </remarks>
		/// <returns> Возвращает время в миллисекундах. <br/>
		///		Если значение <b> time </b> превысит 24 часа, то функция вернёт число превышающее 86'399'999.
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
					System.Windows.MessageBox.Show("Ошибка. class Convert;\nСтрока time = '" + time + "' имеет не верный формат.\n" +
						"Правильный формат для обработки: 'чч:мм:сс'. Пример: time = '12:36:28'.\n return 0;"
					);
					return 0;
                }
				x++; tmp = "";
            }
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> Функция преобразовывает секунды в строку ["чч:мм:сс"] (в сутках 86'400 секунд = [0..86'399]). </summary>
		/// <remarks> 
		///		<b> second: </b> время в секундах. <br/>
		///		<b> h: </b> текстовый разделитель "ч" - часы на выбранном языке. <br/>
		///		<b> m: </b> текстовый разделитель "м" - минуты на выбранном языке. <br/>
		///		<b> s: </b> текстовый разделитель "с" - секунды на выбранном языке. <br/>
		/// </remarks>
		/// <returns> Возвращает время как строка <b>["чч:мм:сс"]</b>. с разделителями на выбранном языке, <br/>
		///		пример: <b>03ч:12м:47с</b>. Если разделители не указывать, строка будет иметь вид: <b>03:12:47</b> <br/>
		///		Если значение <b>second</b> превысит <b>86'399</b>, то функция вернёт время превышающее <b>"23:59:59"</b>. <br/>
		///		Если значение <b>second</b> будет отрицательным, то функция вернёт: <b>"?:?:?"</b>
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
		/// <summary> Функция преобразовывает миллисекунды в строку ["чч:мм:сс:мс"] (в сутках 86'400'000 миллисекунд = [0..86'399'999]). </summary>
		/// <remarks> <b> millisecond: </b> время в миллисекундах. <br/> </remarks>
		/// <returns> Возвращает время как строка ["чч:мм:сс:мс"]. <br/>
		///		Если значение <b> millisecond </b> превысит 86'399'999, то функция вернёт время превышающее "23:59:59:999". <br/>
		///		Если значение <b>millisecond</b> будет отрицательным, то функция вернёт: <b>"??:??:??:???"</b>
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

		/// <summary> Метод форматирует число <b>value</b> добавляя отступы между разрядами. Пример: 1'000'000'000. </summary>
		/// <value>	
		///		<b><paramref name="value"/>:</b> число в формате string. <br/>
		///		<b><paramref name="tab"/>:</b> разделитель. По умолчанию строка разделяется пробелами. <br/>
		/// </value>
		/// <returns> Возвращает отформатированное число в виде строки с пробелами между разрядами: xx'xxx'xxx'xxx. </returns>
		public static string toTABString(string value, string tab = " ") {
			if (value.Length < 4) return value; else {
				char[] chr = new char[value.Length + (value.Length - 1) / 3];	int x = chr.Length; int counter = 0;
                for (int i = value.Length - 1; i >= 0; i--) {
					x--; counter++;	if (counter == 4) { chr[x] += tab[0]; counter = 0; i++; } else chr[x] += value[i];
				} return new string(chr);
			}
		}

		/// <summary> Проверка всех промежуточных разрешений экрана. <br/> Метод проверяет: входит ли текущая высота экрана в диапазон размеров. <br/>	При отсутствии одного из размеров, метод проверит только нижнюю или верхнюю границу. </summary>
		/// <value>	<b><paramref name="min"/>:</b> минимальный размер экрана. <br/>	<b><paramref name="max"/>:</b> максимальный размер экрана. </value>
		/// <returns> Возвращает <b>true</b> если высота экрана текущего разрешения попадает в множество. <br/> Возвращает <b>false</b> если оба параметра оказались <b>default</b> или значения вне диапазона. </returns>
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

		/// <summary> Метод преобразует пользовательское число в компоненту цвета <b>RGB</b>. <br/> Метод проверяет нижнюю и верхнюю границу преобразованного числа. </summary>
		/// <returns> Возвращает компоненту цвета <b>RGB</b> в диапазоне [0..255]. </returns>
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

		/// <summary> Метод проверяет нижнюю/верхнюю границу компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочное значение <b>cl</b> является изменяемым в ходе выполнения кода. </remarks>
		public static void toRGB(ref int cl) { if (cl < 0) cl = byte.MinValue; else if (cl > 255) cl = byte.MaxValue; }
		/// <summary> Метод проверяет нижнюю/верхнюю границу каждой компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочные значения являются изменяемыми в ходе выполнения кода. </remarks>
		public static void toRGB(ref int R, ref int G, ref int B, ref int A) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;	if (A < 0) A = 0; else if (A > 255) A = 255;
		}
		/// <summary> Метод проверяет нижнюю/верхнюю границу каждой компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочные значения являются изменяемыми в ходе выполнения кода. </remarks>
		public static void toRGB (ref int R, ref int G, ref int B) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;
		}
		/// <summary> Метод вычисляет новое промежуточное значение из пары чисел с учётом <b>Alpha</b>. <br/> Эти значениями могут быть как компоненты цвета, так и другие величины, где нужна интерполяция. <br/> Параметр <paramref name="Alpha"/> проверяется на допустимые значения, в случае выхода из диапазона, приводится к нижней или верхней границе. </summary>
		/// <value>
		///		<b><paramref name="First"/>:</b> первое значение, для которого применяется интерполяция <b>Alpha</b>. <br/> <b><paramref name="Second"/>:</b> второе значение, из которого получается возвращаемое. <br/>
		///		<b><paramref name="Alpha"/>:</b> величина интерполяции или прозрачности для значения <paramref name="First"/>, диапазон: <b>[0..1],</b> <br/> где 0 - полностью прозрачный, 1 - 100% не прозрачный, 0.5 - полупрозрачный (если параметры являются компонентами цвета). <br/>
		/// </value>
		/// <returns> Возвращает новое промежуточное значение с учётом <b>Alpha</b>. </returns>
		public static double Interpolate(double First, double Second, double Alpha) {
			Alpha = Alpha < 0 ? 0 : Alpha > 1 ? 1 : Alpha;//[0..1]
			//return First * (Alpha / 255) + Second * (1 - (Alpha / 255));
			return First * Alpha + Second * (1.0 - Alpha);
		}

		/// <summary> Метод конвертирует цветовую модель <b>RGB</b> (Color) в цветовую модель <b>HSV.</b> </summary>
		/// <returns> Возвращает преобразованную цветовую модель <b>HSV.</b> </returns>
		public static HSV ColorToHSV(this Color color) {
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));
			double hue = color.GetHue();
			double saturation = (max == 0) ? 0 : 1.0 - (1.0 * min / max);
			double value = max / 255.0;
			return new HSV(hue, saturation, value);
		}
		/// <summary> Метод конвертирует цветовую модель <b>HSV</b> в цветовую модель <b>RGB</b> (Color). </summary>
		/// <returns> Возвращает преобразованную цветовую модель <b>RGB</b> (Color). </returns>
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