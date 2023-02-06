using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace UFO {
    /// <summary> Класс для хода выполнения вычислений. </summary>
    public class TLoadProcess {
        public enum Report { Show, Hide }
        /// <summary> Ссылка на объект <b>LANGUAGES</b> </summary>
        public WindowsInterface.Form1.TLANGUAGES Languages;
        /// <summary> Измеряет затраченное время на выполнение всего объёма вычислений. </summary>
        private Stopwatch Time = new Stopwatch();

        /// <summary> Панель содержащая текст и ProgressBar для отображения хода выполнения вычисляемых процессов. </summary>
        public Panel LoadPanel;
        /// <summary> Текст с информацией о процессе вычислений. </summary>
        public Label LoadText;
        /// <summary> Текст с информацией о значении <b>LoadProgressBar.Value</b> в процентах. </summary>
        public Label PercentText;
        /// <summary> Полоса закрузки отображающая процент хода вычисляемых процессов. </summary>
        public ProgressBar LoadProgressBar;

        /// <summary> Конструктор класса <b>TLoadProcess</b> с инициализацией некоторых полей. </summary>
        /// <remarks> Внимание! <br/> Сразу после вызова любого конструктора этого класса, следует вызывать инициализатор полей-контролов: <b>LoadProcess_Draw(...);</b> </remarks>
        public TLoadProcess(WindowsInterface.Form1.TLANGUAGES Languages) { this.Languages = Languages; }

        /// <summary> 
        ///     Метод инкрементирует <b>value</b> значение ProgressBar-а и обновляет его во время хода выполнения вычислений. <br/>
        ///     Этот метод следует поместить в тело <b>цикла/рекурсии/другого метода</b> где происходят вычислительные процессы.
        /// </summary>
        public /*async*/ void _Update() {
            LoadProgressBar.Value++; 
            PercentText.Text = Languages.TLoadProcess[1]/*Выполнено:*/ + 
                (int)((double)LoadProgressBar.Value / LoadProgressBar.Maximum * 100.0) + "%";
            LoadText.Left = (LoadPanel.Width - LoadText.Width) / 2;
            PercentText.Left = LoadProgressBar.Left + LoadProgressBar.Width - PercentText.Width;//лепим справа
            Application.DoEvents(); //Refresh(); Update();
            //await Task.Run(() => { Refresh(); });
        }

        ///<summary>
        ///     ИНИЦИАЛИЗАТОР ПОЛЕЙ-КОНТРОЛОВ. ВЫЗЫВАЕТСЯ 1 РАЗ СРАЗУ ПОСЛЕ СОЗДАНИЯ ЭКЗЕМПЛЯРА КЛАССА. <br/>
        ///     Метод создаёт и инициирует панель, текст и индикатор выполнения вычислений <b>(ProgressBar)</b>. <br/>
        ///</summary>
        /// <value> 
        ///     <b> <paramref name="Parent"/>: </b> владелец панели. <br/>
        ///     <b> <paramref name="PanelSize"/>: </b> размер панели. <br/>
        ///     <b> <paramref name="PanelBackColor"/>: </b> цвет фона панели. <br/>
        ///     <b> <paramref name="HeadText"/>: </b> шапка с текстовой информацией над ProgressBar-ом. <br/>
        ///     <b> <paramref name="TextForeColor"/>: </b> цвет шрифта шапки с текстовой информацией над ProgressBar-ом. <br/>
        /// </value>
        public void Init_Controls(Control Parent, Size PanelSize, Color PanelBackColor, string HeadText, Color TextForeColor) {
            LoadPanel = new Panel { Parent = Parent, Visible = false, BackColor = PanelBackColor, Size = PanelSize,
                Location = new Point((Parent.Width - PanelSize.Width) / 2, (Parent.Height - PanelSize.Height) / 2),
            };

            LoadText = new Label { Parent = LoadPanel, Visible = true,
                Text = HeadText, ForeColor = TextForeColor, BackColor = PanelBackColor, AutoSize = true,
            };
            float sz = LoadPanel.Width / 45F; LoadText.SizeFont(sz, FontStyle.Bold);
            LoadText.Left = (LoadPanel.Width - LoadText.Width) / 2;//лепим по центру
            LoadText.Top = (LoadPanel.Height - LoadText.Height) / 2 - 25;//лепим почти по центру

            LoadProgressBar = new ProgressBar { Parent = LoadPanel, Visible = true,
                Width = 500, Height = 25, Minimum = 0, Maximum = 100, Value = 0, Step = 1, MarqueeAnimationSpeed = 0,
            };
            LoadProgressBar.Left = (LoadPanel.Width - LoadProgressBar.Width) / 2;//лепим по центру
            LoadProgressBar.Top = (LoadPanel.Height - LoadProgressBar.Height) / 2 + 25;//лепим почти по центру

            PercentText = new Label { Parent = LoadPanel, Visible = true,
                Text = "", ForeColor = TextForeColor, BackColor = PanelBackColor, AutoSize = true,
                Location = new Point(LoadProgressBar.Left, LoadProgressBar.Top + LoadProgressBar.Height + 5),
            }; LoadText.SizeFont(sz * 0.85F, FontStyle.Regular);

            LoadPanel.BringToFront();
        }

        /// <summary> Метод показывает панель отображающую ход выполнения вычисляемых процессов. </summary>
        /// <value> 
        ///     <b> <paramref name="HeadText"/>: </b> текст с информацией о процессе вычислений. <br/> 
        ///     <b> <paramref name="length"/>: </b> количество вычисляемых порций процессов. <br/> 
        /// </value>
        private void ShowPanel(string HeadText, int length) {
            if (LoadPanel != null) {
                LoadText.Text = HeadText; LoadProgressBar.Value = 0;
                LoadProgressBar.Maximum = length; LoadPanel.Visible = true;
            }
        }

        /// <summary> Суть метода: остановка, обнуление и начало измерения затраченного времени на вычисления. </summary>
        /// <value> <inheritdoc cref="ShowPanel"/> </value>
        public void Restart(string HeadText, int length) { ShowPanel(HeadText, length); Time.Restart(); }
        /// <summary> Суть метода: запуск или возобновление измерения затраченного времени на вычисления. </summary>
        /// <value> <inheritdoc cref="ShowPanel"/> </value>
        public void Start(string HeadText, int length) { ShowPanel(HeadText, length); Time.Start(); }

        /// <summary> Метод скрывает панель отображающую ход выполнения вычисляемых процессов и останавливает измиритель времени. </summary>
        /// <value> 
        ///     <b> <paramref name="MB"/>: </b> <b>enum</b> перечисление. Варианты: <b>Show = 0</b>, <b>Hide = 1</b>. <br/>
        ///     параметр служит для отображения отчёта с технической информацией. <b>MessageBox.Show(...);</b> <br/> 
        /// </value>
        public void Stop(Report MB = Report.Hide) { 
            Time.Stop();//останавливаем секундомер
            if (MB == Report.Show) { MessageBox.Show(
                    Languages.TLoadProcess[2] + "\n\t" +//"Загрузка окончена. Время загрузки:" 
                    Languages.TLoadProcess[3] + " " +/*Прошло*/ Languages.TLoadProcess[4] + " "/*тиков:*/ + Time.ElapsedTicks + "\n\t" +
                    Languages.TLoadProcess[3] + " " +/*Прошло*/ Languages.TLoadProcess[5] + " "/*мс.:*/ + Time.ElapsedMilliseconds + "\n\t" +
                    Languages.TLoadProcess[3] + " " +/*Прошло*/ Languages.TLoadProcess[6] + " "/*сек:*/ + (Time.ElapsedMilliseconds / 1000) + "\n\t" +
                    Languages.TLoadProcess[7] + " " +/*Время:*/ Convert.ToTimeString((int)(Time.ElapsedMilliseconds / 1000)) + "\n" +
                    Languages.TLoadProcess[8]//Для продолжения нажмите 'ОК'.
                );
            }
            if (LoadPanel != null) LoadPanel.Visible = false;
        }
    }
}
