
using System;

namespace UFO {

    /// <summary> Генератор случайных чисел на основе барабана с вариантами инициализации. </summary>
    /// <remarks>
    ///     <b> INIT.RandomNext: </b> упрощённая инициализация с помощью класса System.Random <br/>
    ///     <b> INIT.Memory: </b> инициализация через прогулку по памяти оперативки. <br/>
    ///     <b> INIT.No: </b> барабан будет заполнен нулями. <br/>
    ///     При создании экземпляра без параметра: RANDOM Random = new Random(); <br/>
    ///     класс заполнит барабан по умолчанию нулями.
    /// </remarks>
    public class RANDOM {
        public enum INIT { RandomNext, Memory, No };
        private const uint NSEED = 100000;          //4 byte
        private ushort[] seed = new ushort[NSEED];  //2 byte
        private uint Gs = 0;                        //4 byte
        //public string s = "";

        /// <summary> Конструктор с инициализцией барабана. </summary>
        /// <remarks> 
        ///     Пааметр <b> INITtype </b> может принимать следующие значения: <br/>
        ///     <inheritdoc cref = "RANDOM"> </inheritdoc>
        /// </remarks>
        public RANDOM(INIT INITtype = INIT.No) {
            switch (INITtype) {
			    case INIT.RandomNext: INITIALIZATION_RandomNext(); break;
			    case INIT.Memory: INITIALIZATION_memory(); break;
			    case INIT.No: break;//барабан будет заполнен нулями в массиве seed[] по умолчанию
			}
        }

        /// <summary> Функция генерирует случайное положительное число (unsigned). </summary>
        /// <returns> Возвращает случайное число uint в диапазоне [0 .. ushort.MaxValue]. </returns>
        public uint uRND() {
            Gs++; if (Gs >= NSEED) Gs = 0;
            seed[Gs] = (ushort)(seed[Gs] * (Gs + 13221137) + 123451 - Gs + seed[seed[seed[Gs] % NSEED] % NSEED]);
            return seed[Gs];
        }

        /// <summary> Функция генерирует случайное положительное число (unsigned) с верхним пределом <b> max </b>. </summary>
        /// <value> <b> max: </b> верхний положительный предел случайного числа. <br/> </value>
        /// <returns> Возвращает случайное число uint в диапазоне [0 .. max]. <br/>
        ///     Если <b> max </b> > <b> ushort.MaxValue </b>, функция фернёт <b> ushort.MaxValue. </b>
        /// </returns>
        public uint uRND(uint max) { return uRND() % (max + 1); }

        /// <summary> Функция генерирует случайное число (signed) в диапазоне [min .. max]. </summary>
        /// <value> 
        ///     <b> min: </b> нижняя граница случайного числа. <br/>
        ///     <b> max: </b> верхняя граница случайного числа. <br/>
        /// </value>
        /// <returns> Возвращает случайное число int в диапазоне [+/- min .. +/- max]. <br/>
        ///     При min > max поведение функции нарушится. Проверка не встроена. <br/>
        ///     Корректный ввод: min меньше или равно max.
        /// </returns>
        public int RND(int min, int max) { return (int)uRND() % (max - min + 1) + min; }

        /// <summary> <inheritdoc cref="RANDOM.RND"/> </summary>
        /// <value> <inheritdoc cref="RANDOM.RND"/> </value> 
        /// <returns> Возвращает случайное число int в диапазоне [+/- min .. +/- max]. <br/>
        ///     Осуществляется проверка на корректность ввода. <br/>
        /// </returns>
        public int check_RND(int min, int max) {
            int num; int n; int add; if (min <= max) { n = 1; add = min; } else { n = -1; add = max; }
            num = (max - min + n); if (num == 0) return 0; else return (int)uRND() % num + add;
        }

        /// <summary>
        ///     Упрощённая инициализация с помощью класса System.Random. <br/>
        ///     Барабан заполняется случайными целыми числами от 0 до <b>ushort</b>.MaxValue.
        /// </summary>
        public void INITIALIZATION_RandomNext() {
            Random rnd = new Random(); for (int i = 0; i < NSEED; i++) seed[i] = (ushort)rnd.Next();
        }

        /// <summary>
        ///     Инициализация через прогулку по памяти оперативки. <br/>
        ///     Барабан заполняется случайными числами найденными указателем в оперативной памяти преобразованными в <b>ushort</b>.
        /// </summary>
        /// <value> <b> drum: </b> количество полных оборотов барабана seed[]. один оборот = 100к итераций. </value>
        public void INITIALIZATION_memory(uint drum = 1) {
            if (drum <= 0) drum = 1;
            unsafe  {
                ulong tmp; uint* ptr; uint GS = Gs; ptr = &GS; uint TimeOut = 0;
                for (int d = 0; d < drum; d++) for (int i = 1; i < NSEED; i++) { if (i % 400 == 0) ptr = &GS;
                    while (true) { TimeOut++; if (TimeOut >= 100000) { TimeOut = 0; break; }
                        try { ptr++;
                                //if (i % 100 == 0)  s += "(&ptr) = " + (uint)(&ptr)
                                //    + "  (ptr) = " + (uint)(ptr) + "  (*ptr) = " + (uint)(*ptr) +
                                //    "  (ptr) + (*ptr) = " + ((uint)(ptr) + (*ptr)).ToString() + "\n";
                            *ptr = (uint)(ptr) + (*ptr);
				            if (ptr != null && *ptr != 0) { 
                                Gs = (uint)((Gs * *ptr + (seed[i - 1] + seed[Gs % NSEED] - i)) / TimeOut); break;
                            } else { Gs = (uint)((Gs + (seed[i - 1] + seed[Gs % NSEED] - i)) / TimeOut); break; }
                        } catch (AccessViolationException) { //AccessViolationException
                            Gs = (uint)((Gs + (seed[i - 1] + seed[Gs % NSEED] - i)) / TimeOut); ptr = &GS; break;
                        }
                    }
                    tmp = (ushort)Gs; 
                    uint mod = (uint)((byte)tmp * (ushort)tmp); if (mod == 0) mod = 1;
                    seed[i] = (ushort)(Gs % mod);
                        
                    //if (i % 1000 == 0) s += ("seed[" + d + "][" + i + "] = " + seed[i] + "\n");
                }
            }
		}
	}

}
