
using System;

namespace UFO {

    /// <summary> ��������� ��������� ����� �� ������ �������� � ���������� �������������. </summary>
    /// <remarks>
    ///     <b> INIT.RandomNext: </b> ���������� ������������� � ������� ������ System.Random <br/>
    ///     <b> INIT.Memory: </b> ������������� ����� �������� �� ������ ����������. <br/>
    ///     <b> INIT.No: </b> ������� ����� �������� ������. <br/>
    ///     ��� �������� ���������� ��� ���������: RANDOM Random = new Random(); <br/>
    ///     ����� �������� ������� �� ��������� ������.
    /// </remarks>
    public class RANDOM {
        public enum INIT { RandomNext, Memory, No };
        private const uint NSEED = 100000;          //4 byte
        private ushort[] seed = new ushort[NSEED];  //2 byte
        private uint Gs = 0;                        //4 byte
        //public string s = "";

        /// <summary> ����������� � ������������� ��������. </summary>
        /// <remarks> 
        ///     ������� <b> INITtype </b> ����� ��������� ��������� ��������: <br/>
        ///     <inheritdoc cref = "RANDOM"> </inheritdoc>
        /// </remarks>
        public RANDOM(INIT INITtype = INIT.No) {
            switch (INITtype) {
			    case INIT.RandomNext: INITIALIZATION_RandomNext(); break;
			    case INIT.Memory: INITIALIZATION_memory(); break;
			    case INIT.No: break;//������� ����� �������� ������ � ������� seed[] �� ���������
			}
        }

        /// <summary> ������� ���������� ��������� ������������� ����� (unsigned). </summary>
        /// <returns> ���������� ��������� ����� uint � ��������� [0 .. ushort.MaxValue]. </returns>
        public uint uRND() {
            Gs++; if (Gs >= NSEED) Gs = 0;
            seed[Gs] = (ushort)(seed[Gs] * (Gs + 13221137) + 123451 - Gs + seed[seed[seed[Gs] % NSEED] % NSEED]);
            return seed[Gs];
        }

        /// <summary> ������� ���������� ��������� ������������� ����� (unsigned) � ������� �������� <b> max </b>. </summary>
        /// <value> <b> max: </b> ������� ������������� ������ ���������� �����. <br/> </value>
        /// <returns> ���������� ��������� ����� uint � ��������� [0 .. max]. <br/>
        ///     ���� <b> max </b> > <b> ushort.MaxValue </b>, ������� ����� <b> ushort.MaxValue. </b>
        /// </returns>
        public uint uRND(uint max) { return uRND() % (max + 1); }

        /// <summary> ������� ���������� ��������� ����� (signed) � ��������� [min .. max]. </summary>
        /// <value> 
        ///     <b> min: </b> ������ ������� ���������� �����. <br/>
        ///     <b> max: </b> ������� ������� ���������� �����. <br/>
        /// </value>
        /// <returns> ���������� ��������� ����� int � ��������� [+/- min .. +/- max]. <br/>
        ///     ��� min > max ��������� ������� ���������. �������� �� ��������. <br/>
        ///     ���������� ����: min ������ ��� ����� max.
        /// </returns>
        public int RND(int min, int max) { return (int)uRND() % (max - min + 1) + min; }

        /// <summary> <inheritdoc cref="RANDOM.RND"/> </summary>
        /// <value> <inheritdoc cref="RANDOM.RND"/> </value> 
        /// <returns> ���������� ��������� ����� int � ��������� [+/- min .. +/- max]. <br/>
        ///     �������������� �������� �� ������������ �����. <br/>
        /// </returns>
        public int check_RND(int min, int max) {
            int num; int n; int add; if (min <= max) { n = 1; add = min; } else { n = -1; add = max; }
            num = (max - min + n); if (num == 0) return 0; else return (int)uRND() % num + add;
        }

        /// <summary>
        ///     ���������� ������������� � ������� ������ System.Random. <br/>
        ///     ������� ����������� ���������� ������ ������� �� 0 �� <b>ushort</b>.MaxValue.
        /// </summary>
        public void INITIALIZATION_RandomNext() {
            Random rnd = new Random(); for (int i = 0; i < NSEED; i++) seed[i] = (ushort)rnd.Next();
        }

        /// <summary>
        ///     ������������� ����� �������� �� ������ ����������. <br/>
        ///     ������� ����������� ���������� ������� ���������� ���������� � ����������� ������ ���������������� � <b>ushort</b>.
        /// </summary>
        /// <value> <b> drum: </b> ���������� ������ �������� �������� seed[]. ���� ������ = 100� ��������. </value>
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
