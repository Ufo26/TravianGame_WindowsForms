using GameLogica;
using Global_Var;
using UFO;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using static GameLogica.TGame;
using static GameLogica.Enums_and_structs;
using static UFO.Convert;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsInterface {
    interface IForm1 {
        /// <summary> Метод Form1. Срабатывает один раз при создании формы Form1. </summary>
        void Form1_Load(object sender, EventArgs e);//загрузка формы на старте
        /// <summary> Метод Form1. Обрабатывает изменения размеров главной формы. </summary>
        void Form1_Resize(object sender, EventArgs e);
        /// <summary> Метод Form1. Обрабатывает изменения локации главной формы. </summary>
        void Form1_LocationChanged(object sender, EventArgs e);

        /// <summary> Метод Timer. Анимирует разворачивания/сворачивания RichTextBox с информацией о нации в главном меню. </summary>
        void tm_Animate_MainMenu_Tick(object sender, EventArgs e);
        /// <summary> Метод Timer. Анимирует разворачивания/сворачивания панели с информацией о войсках в деревне. </summary>
        void tm_Animate_Panel_Army_Tick(object sender, EventArgs e);

        /// <summary> Метод срабатывает при изменении вкладки в tabControl. <b>Event tabControl.</b> </summary>
        void tabControl_SelectedIndexChanged(object sender, EventArgs e);
        /// <summary> Метод обрабатывает изменения вкладок в tabControl. </summary>
        void TabControl_SelectedIndexChanged();

        /// <summary> Универсальный обработчик для любых контролов, на которых можно рисовать. </summary>
        /// <remarks> Метод обрабатывает отрисовку на поверхности контрола. </remarks>
        void onPaint(object sender, PaintEventArgs e);

        /// <summary> Универсальный обработчик для любых контролов. </summary>
        /// <remarks> Метод обрабатывает одиночный клик левой кнопкой мыши по видимой части контрола. </remarks>
        void Control_DoubleClick(object sender, EventArgs e);
        /// <summary> <inheritdoc cref = "Control_DoubleClick"> </inheritdoc> </summary>
        /// <remarks> Метод обрабатывает одиночный клик левой кнопкой мыши по видимой части контрола. </remarks>
        void Control_Click(object sender, EventArgs e);
        /// <summary> <inheritdoc cref = "Control_Click"> </inheritdoc> </summary>
        /// <remarks> Метод обрабатывает наведение курсора мыши на видимую часть контрола. </remarks>
        void Control_MouseEnter(object sender, EventArgs e);
        /// <summary> <inheritdoc cref = "Control_Click"> </inheritdoc> </summary>
        /// <remarks> Метод обрабатывает выход за пределы видимой части контрола курсором мыши. </remarks>
        void Control_MouseLeave(object sender, EventArgs e);
        /// <summary> <inheritdoc cref = "Control_Click"> </inheritdoc> </summary>
        /// <remarks> Метод обрабатывает движение курсора мыши по видимой части контрола. </remarks>
        void Control_MouseMove(object sender, MouseEventArgs e);

        ///<summary> Метод задаёт начальные размеры, локации и прочие стартовые параметры для интерфейса. </summary>
        void StartingSettingsOfControls();
        ///<summary> Метод подсвечивает выбранное ресурсное поле во вкладке [1] <br/> при наведении курсором на область выделения.</summary>
        ///<remarks>
        ///     <b> left/top </b> - позиция подсвеченного Bitmap рисунка на фоне вкладки [1]. <br/>
        ///     <b> width/height </b> - размер подсвеченного Bitmap рисунка. <br/>
        ///     <b> MIN/MAX </b> - нижний и верхний порог цветового фильтра. <br/>
        ///     <b> Alpha </b> - альфа канал [0..1]. Степень смешения подсветки. 0 - прозрачный 100%, 1 - не прозрачный 100%. <br/>
        ///     <b> mod </b> - модификатор. 0: 0.5 Alpha. 1: градации серого. 2: градации красного. <br/>
        ///     <b> num </b> - обработка каждого num пикселя.
        ///</remarks>
        void Illumination(int left, int top, int width, int height, Color MIN, Color MAX, double Alpha, byte mod, byte num);
    }

    public partial class Form1 : Form, IForm1 {
        /// <summary>
        ///     <b> LC: </b> Location Controls <br/>
        ///     Здесь хранятся все коэффициенты координат контроллов на форме в диапазоне [0..1], где 0 = левый край, 0.5 = центр, 1 = правый край. <br/>
        ///     Далее эти числа преобразовываются под текущее разрешение экрана методом <b> Control.Location(); </b> <br/>
        ///     Данные координаты корректны для вкладки "ресурсные поля". Во вкладке "деревня" координаты некоторых контролов отличаются.
        /// </summary>
        private static class LC {//Location Controls
            ///<summary> панель информации о деревне 1 </summary>
            public static double pv1_X = 0.0275;//55;
            /// <inheritdoc cref="pv1_X"> </inheritdoc>
            public static double pv1_Y = 0.1900;//174;
            ///<summary> панель информации о деревне 2 </summary>
            public static double pv2_X = pv1_X;//55;
            /// <inheritdoc cref="pv2_X"> </inheritdoc>
            public static double pv2_Y = 0.3405;//314;
            /// <summary> панель добычи ресурсов </summary>
            public static double prp_X = 0.5780;//1100;
            /// <inheritdoc cref="prp_X"> </inheritdoc>
            public static double prp_Y = 0.3600;//350;
            /// <summary> панель передвижения войск </summary>
            public static double pmu_X = 0.5780;//1100;
            /// <inheritdoc cref="prp_X"> </inheritdoc>
            public static double pmu_Y = 0.1470;//120;
            /// <summary> панель строительства построек </summary>
            public static double pc_X = 0.2400;//480;
            /// <inheritdoc cref="prp_X"> </inheritdoc>
            public static double pc_Y = 0.7500;//810;
            /// <summary> картинка нации выбранного / созданного аккаунта </summary>
            public static double pe_X = 0.7765;//1484;
            /// <inheritdoc cref="pe_X"> </inheritdoc>
            public static double pe_Y = 0.5140;//466;
            /// <summary> текст нации выбранного / созданного аккаунта </summary>
            public static double le_X = pe_X + 0.015;//20;
            /// <inheritdoc cref="le_X"> </inheritdoc>
            public static double le_Y = 0.3890;//356;
            /// <summary> картинка с картой </summary>
            public static double pm_X = 0.1700;//505;
            /// <inheritdoc cref="pm_X"> </inheritdoc>
            public static double pm_Y = 0.3260;//295;
            /// <summary> позиция GroupBox на форме </summary>
            public static double gb_X = 0.015;//20;
            /// <inheritdoc cref="pm_X"> </inheritdoc>
            public static double gb_Y = 0.5500;//594;
            /// <summary> панель списка армий </summary>
            public static double pa_X = 0.5780;//1100;
            /// <inheritdoc cref="prp_X"> </inheritdoc>
            public static double pa_Y = 0.6100;//658;
            /// <summary> кнопка смены фоновой картинки на вкладке "Деревня" </summary>
            public static double bnbv_X = 0.015;
            /// <inheritdoc cref="bnbv_X"> </inheritdoc>
            public static double bnbv_Y = 0.155;
        }

        /// <summary> Таймер в общем потоке. Управляет фронтендом: анимацией, визуальынми контролами, интерфейсом и т.д. </summary>
        public System.Windows.Forms.Timer Event_Timer_Frontend = null;
        /// <summary> Таймер в отдельном фоновом потоке. Управляет бэкендом: тиками таймеров событий, генерацией отчётов, завершением построек, расчётами битв и т.д. </summary>
        public System.Threading.Timer Event_Thread_Timer_Backend = null;

        /// <summary> Экземпляр класса "TGame". Всё что нужно для игры. </summary>
        public TGame GAME = Newtonsoft.Json.JsonConvert.DeserializeObject<TGame>
               (File.ReadAllText("DATA_BASE/JSON/Default_Game.json"));//загрузить в объект json строку из файла
        /// <summary> Экземпляр класса "TPlayer". Всё что нужно для аккаунта и его деревень. </summary>
        public TPlayer Player = new TPlayer();
        /// <summary> Содержит список всех ботов данного аккаунта. <br/> Каждая строка <b> BotList </b> является объектом класса <b> TPlayer </b> <br/> </summary>
        public List<TPlayer> BotList = new List<TPlayer>();

        /// <summary> Общий генератор случайных чисел для form1. </summary>
        private static readonly RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);
        /// <summary> массив всплывающих подсказок. </summary>
        private ToolTip[] tool_tip = null;

        /// <summary> Подсветка выбранного ресурсного поля в определённых координатах bg_Resources. </summary>
        private PictureBox Selected_Cell = null;

        /// <summary>
        ///     Класс с элементами отображения хода загрузки процесса. <br/>
        ///     Состоит из полей-переменных класса для логики и полей-контролов для визуализации.
        /// </summary>
        public TLoadProcess LoadProcess;

        /// <summary> Альтернативные прогресс бары при переполнении хранилищ. </summary>
        public Button AlternativeProgress_wood = new Button(), AlternativeProgress_clay = new Button(), AlternativeProgress_iron = new Button(),
                      AlternativeProgress_crop = new Button(), AlternativeProgress_cons_crop = new Button(), AlternativeProgress_gold = new Button();

        /// <summary> Массив кнопок для слотов ресурсных полей [0..17]. Всего 40 шт. [0..39] : [0..17] + [0..21] </summary>
        private Button[] Button_Slot_Resource = null;
        /// <summary> Массив кнопок для слотов деревенских построек [0..21]. Всего 40 шт. [0..39] : [0..17] + [0..21] </summary>
        private Button[] Button_Slot_Builds = null;
        /// <summary> Массив картинок для слотов деревенских построек [0..21] = 22 шт. </summary>
        private PictureBox[] img_Slot_builds = null;
        /// <summary> Массив координат кнопок относительно Parent = bg_Village. </summary>
        private Point[] Button_Slot_Location = null;

        /// <summary> Пользовательская отрисовка всплывающей подсказки. </summary>
        /// <remarks> фон-панель всплывающей подсказки. </remarks>
        private PictureBox pb_tool_tip_BackGround = null;
        /// <summary> <inheritdoc cref="pb_tool_tip_BackGround"/> </summary>
        /// <remarks> массив пиктограмм. Размер массива = 4 </remarks>
        private PictureBox[] pb_tt_pr = null;
        /// <summary> <inheritdoc cref="pb_tool_tip_BackGround"/> </summary>
        /// <remarks> массив величин ресурсов для пиктограмм. Размер массива = 4 </remarks>
        private Label[] lb_tt_pr = null;
        /// <summary> <inheritdoc cref="pb_tool_tip_BackGround"/> </summary>
        /// <remarks> массив названия и описания. Размер массива = 2 </remarks>
        private Label[] lb_tt_info = null;

        public Form2_CreateAccaunt form2;

        public Form1() {
            InitializeComponent();

            form2 = new Form2_CreateAccaunt(this);//создаём форму 2

            //компоненты которые можно перетаскивать на форме:
            ControlMover.Add(GroupBox_Village);//панель со списком деревень
            //если двигать, будет косяк при вычислении высоты панели передвижения юнитов
            //ControlMover.Add(Panel_Move_Units);//панель передвижения юнитов 
            //если двигать, будет косяк при вычислении высоты панели передвижения юнитов
            //ControlMover.Add(Panel_Resources_Production);//панель с добычей ресурсов
            ControlMover.Add(Panel_Army);//панель с отображением армии
            ControlMover.Add(Panel_Construction);//панель строительства построек

            //обработчик события Scroll для таблицы армии. Запоминает позицию вертикального скролла
            grid_Army.Scroll += (s, e) => { FDSRI = e.NewValue; Text = Random.RND(0, 100).ToString(); };
        }

        //================================================ СТАРТОВАЯ ЗАГРУЗКА ФОРМЫ ================================================
        public void Form1_Load(object sender, EventArgs e) {
            //MessageBox.Show(DateTime.Now.ToString());//вывод даты и времени
            Cursor.Current = Cursors.WaitCursor;
            if (File.Exists($"{GAME.PathFolderSave}/Interface.DAT")) GAME.LoadInterface(GAME.PathFolderSave);/*загрузка интерфейса*/
            Global.Screen_Width = Screen.PrimaryScreen.Bounds.Size.Width;
            Global.Screen_Height = Screen.PrimaryScreen.Bounds.Size.Height;
            Global.ScreenArea_Width = Screen.PrimaryScreen.WorkingArea.Size.Width;
            Global.ScreenArea_Height = Screen.PrimaryScreen.WorkingArea.Size.Height;
            Global.MainWindowWidth = Global.ScreenArea_Width; Global.MainWindowHeight = Global.ScreenArea_Height; //размер главного окна
            Name = "MainForm"; //MaximizeBox = false; 
            Text = "C# / Travian Game Offline Version / Window: " + Width + "x" + Height + " / Screen: " + Global.Screen_Width + "x" + Global.Screen_Height;
            tabControl.Location = new Point(0, menuStrip1.Top + menuStrip1.Height);
            tabControl.Size = new Size(ClientSize.Width, ClientSize.Height - menuStrip1.Height - statusStrip.Height);

            //вычисляем новые размеры и местоположения всех контролов относительно текущего разрешения экрана
            btn_Teutons.Top = btn_Rome.Top = btn_Gauls.Top = ToCSR(200);
            btn_Teutons.ReSize(); btn_Rome.ReSize(); btn_Gauls.ReSize();
            btn_Teutons.ReSizeFont(FontStyle.Bold); btn_Rome.ReSizeFont(FontStyle.Bold); btn_Gauls.ReSizeFont(FontStyle.Bold);
            rich_info.ReSize(); rich_info.ZoomFactor = ToCSR(1F);
            pic_wood.ReSize(); pic_clay.ReSize(); pic_iron.ReSize(); pic_crop.ReSize(); pic_cons_crop.ReSize(); pic_gold.ReSize();

            btn_Teutons.Left = (btn_Teutons.Parent.Width - btn_Teutons.Width) / 2;//лепим по центру
            btn_Rome.Left = (btn_Teutons.Left - btn_Rome.Width) / 2;//лепим слева
            btn_Gauls.Left = btn_Gauls.Parent.Width - ((btn_Teutons.Left + btn_Rome.Width) / 2);//лепим справа
            rich_info.Left = (rich_info.Parent.Width - rich_info.Width) / 2;//лепим по центру
            rich_info.Top = btn_Teutons.Top + btn_Teutons.Height + 10;
            bg_Menu.Size = tabControl.TabPages[0].Size;       bg_Resources.Size = tabControl.TabPages[0].Size;
            bg_Village.Size = tabControl.TabPages[0].Size;    bg_Map.Size = tabControl.TabPages[0].Size;
            bg_Village.BackgroundImage = Image.FromFile("DATA_BASE/IMG/BackGround/village_1503x829_" + GAME.bgVillage_NumberImage + ".png");
            bg_Statistics.Size = tabControl.TabPages[0].Size; bg_Report.Size = tabControl.TabPages[0].Size;
            bg_Message.Size = tabControl.TabPages[0].Size;    bg_BuildingTree_T4.Size = tabControl.TabPages[0].Size;
            statusStrip.Top = tabControl.Top + tabControl.Height; statusStrip.Width = Width;
            btn_Map_Left_X.ContextMenuStrip = contextMenuStrip1; btn_Map_Right_X.ContextMenuStrip = contextMenuStrip1;
            btn_Map_Up_Y.ContextMenuStrip = contextMenuStrip1;   btn_Map_Down_Y.ContextMenuStrip = contextMenuStrip1;
            for (int i = 0; i < statusStrip.Items.Count; i++) statusStrip.Items[i].Text = "";

            SetLanguage();//перевести весь текст
            GAME.Event_Stack.Languages = LANGUAGES;
            TSM1.Load_SubMenu_Language_FromFolder(GAME.Language, SelectionOfLanguage_TSMI_Click, LANGUAGES);//создать меню языков
            TSM_0_Item_0.Load_SubMenu_Account_FromFolder(ContinueGameTSMI_Click);//создать меню доступных аккаунтов

            //ПЕРЕОПРЕДЕЛЕНИЕ Parent И BringToFront() ДЛЯ ЭЛЕМЕНТОВ ИНТЕРФЕЙСА
            //кнопка на портрете героя
            btn_Hero.Parent = bg_Village; btn_Hero.BringToFront();
            //кнопка смены фоновой картинки на вкладке "Деревня"
            btn_NextbgVillage.Parent = bg_Village; btn_NextbgVillage.BringToFront();
            //кнопки переключения между вкладками
            pb_Button_Resourses.Parent = bg_Resources; pb_Button_Resourses.BringToFront(); pb_Button_Village.Parent = bg_Resources; pb_Button_Village.BringToFront();
            pb_Button_Map.Parent = bg_Resources; pb_Button_Map.BringToFront(); pb_Button_Statistics.Parent = bg_Resources; pb_Button_Statistics.BringToFront();
            pb_Button_Reports.Parent = bg_Resources; pb_Button_Reports.BringToFront(); pb_Button_Messages.Parent = bg_Resources; pb_Button_Messages.BringToFront();
            //панель-полоска с ресурсами на складе
            Panel_Resources.Parent = bg_Resources; Panel_Resources.BringToFront();
            //панель выработки ресурсов в час
            Panel_Resources_Production.Parent = bg_Resources; Panel_Resources_Production.BringToFront();
            grid_Resources_Production.Parent = bg_Resources; grid_Resources_Production.BringToFront();
            //панели с информацией о деревне
            Panel_Village1.Parent = bg_Resources; Panel_Village1.BringToFront(); Panel_Village2.Parent = bg_Resources; Panel_Village2.BringToFront();
            //панель списка деревень
            GroupBox_Village.Parent = bg_Resources; GroupBox_Village.BringToFront(); GroupBox_Village.MouseDoubleClick += Control_DoubleClick;
            //картинка + текст этноса аккаунта
            Picture_Ethnos.Parent = bg_Resources; Picture_Ethnos.BringToFront(); lb_Ethnos.Parent = bg_Resources; lb_Ethnos.BringToFront();
            //панель передвижения юнитов
            Panel_Move_Units.Parent = bg_Resources; Panel_Move_Units.BringToFront();
            //панель строительства построек
            Panel_Construction.Parent = bg_Resources; Panel_Construction.BringToFront();
            //панель списка войск в деревне
            lb_Army_Header.Parent = Panel_Army; Panel_Army.Parent = bg_Resources; Panel_Army.BringToFront(); 
            grid_Army.Parent = bg_Resources; grid_Army.BringToFront();

            //интерфейс вкладки КАРТА / MAP (очерёдность BringToFront() только такая!!!)
            pb_MAP.Parent = bg_Map;              pb_MAP.BringToFront();              pnl_Map_Coord.Parent = bg_Map; pnl_Map_Coord.BringToFront();
            pnl_Map_Input_Coord.Parent = bg_Map; pnl_Map_Input_Coord.BringToFront(); pnl_Map_DATA.Parent = bg_Map;  pnl_Map_DATA.BringToFront();
            pb_XXXX_Map.Parent = pb_MAP;         pb_XXXX_Map.BringToFront();         pnl_Map_Size.Parent = pb_MAP;  pnl_Map_Size.BringToFront();
            btn_Map_Full_Screen.Parent = pb_MAP; btn_Map_Full_Screen.BringToFront(); pnl_Map_Info_Village_Population.Parent = bg_Map; pnl_Map_Info_Village_Population.BringToFront();
            btn_Map_Left_X.Parent = pb_XXXX_Map; btn_Map_Left_X.BringToFront();      btn_Map_Right_X.Parent = pb_XXXX_Map; btn_Map_Right_X.BringToFront();
            btn_Map_Up_Y.Parent = pb_XXXX_Map;   btn_Map_Up_Y.BringToFront();        btn_Map_Down_Y.Parent = pb_XXXX_Map; btn_Map_Down_Y.BringToFront();
            //создаём массив кнопок для слотов ресурсных полей и построек. 
            Color MDBC_wood = Color.FromArgb(135, 145, 60); Color BC_wood = Color.FromArgb(67, 72, 30);
            Color MDBC_clay = Color.FromArgb(170, 135, 90); Color BC_clay = Color.FromArgb(85, 67, 45);
            Color MDBC_iron = Color.FromArgb(135, 135, 135); Color BC_iron = Color.FromArgb(67, 67, 67);
            Color MDBC_crop = Color.FromArgb(210, 200, 100); Color BC_crop = Color.FromArgb(105, 100, 50);
            
            //Button_Slot_Resource + Button_Slot_Builds = 40 кнопок для всех слотов аккаунта: промышленность + инфраструктура + военные постройки
            Button_Slot_Resource = new Button[18];//ресурсные кнопки [0..17] - length : 18
            Button_Slot_Builds = new Button[22];//деревенские кнопки [0..21] - length : 22
            img_Slot_builds = new PictureBox[Button_Slot_Builds.Length];//картинки для каждого слота в деревне [0..21] - length : 22
            Button_Slot_Location = new Point[Button_Slot_Builds.Length];//массив координат [0..21] - length : 22
            Color MDBC = Color.Black; Color BC = Color.White; float size = Global.Screen_Height / 151F; FontStyle fs;
            if (Global.Screen_Height <= 720) fs = FontStyle.Regular; else fs = FontStyle.Bold;
            for (int i = 0; i < Button_Slot_Resource.Length; i++) {
                //создание ресурсных кнопок
                Button_Slot_Resource[i] = new Button {
                    Parent = bg_Resources, Name = "Button_Slot_resources", Width = 40, Height = 38,
                    Font = new Font(Font.FontFamily, size, fs), Tag = i,/*номер слота*/
                }; switch (i) {
                    //ноутбук 1366х768
                    //цвет верный если это 6-ка
                    case 0: Button_Slot_Resource[i].Location(0.3735, 0.2775); MDBC = MDBC_wood; BC = BC_wood; break;//дерево
                    case 1: Button_Slot_Resource[i].Location(0.434, 0.2785); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 2: Button_Slot_Resource[i].Location(0.479, 0.2932); MDBC = MDBC_wood; BC = BC_wood; break;//дерево
                    case 3: Button_Slot_Resource[i].Location(0.335, 0.325); MDBC = MDBC_iron; BC = BC_iron; break;//железо
                    case 4: Button_Slot_Resource[i].Location(0.4102, 0.3404); MDBC = MDBC_clay; BC = BC_clay; break;//глина
                    case 5: Button_Slot_Resource[i].Location(0.448, 0.348); MDBC = MDBC_clay; BC = BC_clay; break;//глина
                    case 6: Button_Slot_Resource[i].Location(0.295, 0.386); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 7: Button_Slot_Resource[i].Location(0.349, 0.386); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 8: Button_Slot_Resource[i].Location(0.4755, 0.387); MDBC = MDBC_iron; BC = BC_iron; break;//железо
                    case 9: Button_Slot_Resource[i].Location(0.5048, 0.3473); MDBC = MDBC_iron; BC = BC_iron; break;//железо
                    case 10: Button_Slot_Resource[i].Location(0.534, 0.387); MDBC = MDBC_iron; BC = BC_iron; break;//железо
                    case 11: Button_Slot_Resource[i].Location(0.3005, 0.4595); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 12: Button_Slot_Resource[i].Location(0.349, 0.448); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 13: Button_Slot_Resource[i].Location(0.5205, 0.454); MDBC = MDBC_crop; BC = BC_crop; break;//зерно
                    case 14: Button_Slot_Resource[i].Location(0.4395, 0.4912); MDBC = MDBC_wood; BC = BC_wood; break;//дерево
                    case 15: Button_Slot_Resource[i].Location(0.3695, 0.5565); MDBC = MDBC_clay; BC = BC_clay; break;//глина
                    case 16: Button_Slot_Resource[i].Location(0.430, 0.562); MDBC = MDBC_wood; BC = BC_wood; break;//дерево
                    case 17: Button_Slot_Resource[i].Location(0.4905, 0.5348); MDBC = MDBC_clay; BC = BC_clay; break;//глина
                }
                Button_Slot_Resource[i].Button_Styles(Color.Transparent, MDBC, Color.Transparent, Color.Red, Color.Black, 0);
                Button_Slot_Resource[i].ReSize(); Button_Slot_Resource[i].MouseClick += Control_Click;
                Button_Slot_Resource[i].MouseEnter += Control_MouseEnter; Button_Slot_Resource[i].MouseLeave += Control_MouseLeave;
            }
            for (int i = 0; i < Button_Slot_Builds.Length; i++) {
                img_Slot_builds[i] = new PictureBox { //создание деревенских картинок
                    Parent = bg_Village, Name = "img_Slot_builds", BackgroundImageLayout = ImageLayout.Stretch, Tag = i + 18,/*номер слота*/
                    Size = new Size(100, 100),
                }; 
                Button_Slot_Builds[i] = new Button { //создание деревенских кнопок
                    Parent = bg_Village, Name = "Button_Slot_builds", Text = "", Tag = i + 18,/*номер слота*/
                    Font = new Font(Font.FontFamily, size, fs), BackgroundImageLayout = ImageLayout.Stretch,
                    BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/id_build.png"), BackColor = Color.Transparent,
                }; 
                Button_Slot_Builds[i].Size(Button_Slot_Builds[i].BackgroundImage.Width, Button_Slot_Builds[i].BackgroundImage.Height);
                if (i == 0) Button_Slot_Builds[i].Parent = img_Slot_builds[i];//кладём кнопку стены на картинку стены
                else /*если не стена*/ {
                    img_Slot_builds[i].Parent = img_Slot_builds[0];//кладём все картинки на стенку
                    Button_Slot_Builds[i].Parent = img_Slot_builds[i];//кладём все кнопки на свои картинки
                }
                switch (i + 18) {
                    /*стена       i = 0*/case 18: img_Slot_builds[i].Size = new Size(990, 700);//Size(915, 640);
                        //img_Slot_builds[i].Location(0.199, 0.1410); Button_Slot_Builds[i].Location(0.208, 0.691); break;
                        img_Slot_builds[i].Location(0.238, 0.1415); Button_Slot_Builds[i].Location(0.5, 0.840); break;

                    /*пункт сбора i = 1*/case 19: img_Slot_builds[i].Size = new Size(140, 158 + 26); break;
                    /*глав.здание i = 9*/case 27: img_Slot_builds[i].Size = new Size(100, 100); img_Slot_builds[i].Location(0.477, 0.225); break;
                    //безымянные слоты  i = [2..8, 10..21]
                    case 20: img_Slot_builds[i].Location(0.2210, 0.1760); break; case 21: img_Slot_builds[i].Location(0.3220, 0.1100); break;
                    case 22: img_Slot_builds[i].Location(0.4505, 0.0660); break; case 23: img_Slot_builds[i].Location(0.5650, 0.1270); break;
                    case 24: img_Slot_builds[i].Location(0.6700, 0.1955); break; case 25: img_Slot_builds[i].Location(0.1200, 0.2390); break;
                    case 26: img_Slot_builds[i].Location(0.3220, 0.2540); break; case 28: img_Slot_builds[i].Location(0.7880, 0.2845); break;
                    case 29: img_Slot_builds[i].Location(0.1050, 0.3880); break; case 30: img_Slot_builds[i].Location(0.6870, 0.5310); break;
                    case 31: img_Slot_builds[i].Location(0.2310, 0.3970); break; case 32: img_Slot_builds[i].Location(0.6870, 0.3675); break;
                    case 33: img_Slot_builds[i].Location(0.7880, 0.4570); break; case 34: img_Slot_builds[i].Location(0.2210, 0.5390); break;
                    case 35: img_Slot_builds[i].Location(0.3330, 0.4380); break; case 36: img_Slot_builds[i].Location(0.5700, 0.6090); break;
                    case 37: img_Slot_builds[i].Location(0.3400, 0.6180); break; case 38: img_Slot_builds[i].Location(0.4370, 0.4615); break;
                    case 39: img_Slot_builds[i].Location(0.4480, 0.6300); break;
                }
                img_Slot_builds[i].ReSize();
                if (i > 0) Button_Slot_Location[i] = Button_Slot_Builds[i].Centering();//записываем коры кнопки. использую в Update_Buttons_Level_Slots();
                Button_Slot_Builds[i].Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent, Color.Red, Color.Black, 0);
                Button_Slot_Builds[i].MouseClick += Control_Click; Button_Slot_Builds[i].MouseEnter += Control_MouseEnter; Button_Slot_Builds[i].MouseLeave += Control_MouseLeave;
            }

            //создание панели отображающей ход выполнения вычислений
            LoadProcess = new TLoadProcess(LANGUAGES);
            LoadProcess.Init_Controls(this, new Size((int)(Global.MainWindowWidth * 0.5), (int)(Global.MainWindowHeight * 0.5)),
                Color.Black, LANGUAGES.TLoadProcess[0]/*Пустой объект*/, Color.YellowGreen);

            StartingSettingsOfControls();//настройка стартовых свойств некоторых контролов
            //грузим случайный фон в главное меню
            bg_Menu.BackgroundImage = LoadRandom_BackgroundImage();
            //грузим фон на вкладке "Дерево построек"
            bg_BuildingTree_T4.BackgroundImage = Image.FromFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/BuildingTree_T4.png");
            bg_BuildingTree_T4.Height = bg_BuildingTree_T4.BackgroundImage.Height;
            tabPage8.AutoScroll = true;

            //Создаём таймер в основном потоке - бесконечный цикл обработки событий, но не запускаем его
            Event_Timer_Frontend = new System.Windows.Forms.Timer { Enabled = false, Interval = (int)(GAME.SpeedGame / 2) };
            Event_Timer_Frontend.Tick += CallBack_EventHandler_Frontend;
            //Создаём таймер в отдельном фоновом потоке - бесконечный цикл обработки событий, но не запускаем его
            Event_Thread_Timer_Backend = new System.Threading.Timer(CallBack_EventHandler_Backend, GAME, Timeout.Infinite, (int)GAME.SpeedGame);

            //настраиваем всплывающие подсказки
            for (int i = 0; i < tool_tip.Length; i++) if (tool_tip[i] != null) tool_tip[i].Active = GAME.ToolTipFlag;
            Enable_Visible_Tooltips_TSMI.Checked = GAME.ToolTipFlag;//настраиваем чек всплывающих подсказок
            Enable_Visible_ColorArmy_TSMI.Checked = GAME.ColorArmyFlag;//настраиваем чек армии (цветные/монохромные)
            //ставим метки на чеках. если их ставить в конструкторе форм, юудут глюки
            Enable_Visible_Tooltips_TSMI.Tag = 0;    Enable_Visible_ColorArmy_TSMI.Tag = 1;

            Cursor.Current = Cursors.Default;
        }
        //================================================ СТАРТОВАЯ ЗАГРУЗКА ФОРМЫ ================================================

        public void Form1_Resize(object sender, EventArgs e) {
            Size = new Size(Global.MainWindowWidth, Global.MainWindowHeight);
            int HeadBar_Height = Global.ScreenArea_Height - ClientSize.Height;
            tabControl.Width = Width; tabControl.Height = Global.MainWindowHeight - HeadBar_Height - tabControl.Top - statusStrip.Height;
            statusStrip.Top = tabControl.Top + tabControl.Height; Text = "C# / Travian Game Offline Version / Window: " + Size.Width + "x" + Size.Height + " / Screen: " + Global.Screen_Width + "x" + Global.Screen_Height;
        }
        public void Form1_LocationChanged(object sender, EventArgs e) { Location = new Point(0, 0); }

        //========================================================================================================================
        //                                              БЛОК: ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        //========================================================================================================================
        /// <summary> Метод ищет в папке <b>BackGround</b> случайную картинку для фона. </summary>
        /// <returns> Возвращает случайную картинку произвольного формата из папки <b>BackGround</b>. <br/> Если картинку загрузить не удалось, возвращает <b>null.png</b> <br/> Если картинки <b>null.png</b> тоже нет, возвращает <b>null</b> <br/> Если <b>null</b> тоже нет, тут мои полномочия всё, приехали. </returns>
        private Image LoadRandom_BackgroundImage() {
            string PATH = "DATA_BASE/IMG/BackGround"; var path = Directory.GetFiles(PATH + "/MainMenu/");
            List<string> list = new List<string>(); 
            if (path != null) for (int i = 0; i < path.Length; i++) {
                string ext = path[i].Substring(path[i].Length - 4, 4).ToUpper();
                if (ext == ".JPG" || ext == "JPEG" || ext == ".PNG" || ext == ".BMP" || ext == ".GIF" || ext == "TIFF")
                    list.Add(path[i]);
            } if (list.Count > 0) return Image.FromFile(list[Random.RND(0, list.Count - 1)]);
            else if (File.Exists($"{PATH}/null.png")) return Image.FromFile($"{PATH}/null.png"); else return null;
        }

        public void StartingSettingsOfControls() {
            float FSize = ToCSR(10);/*размер шрифта*/ 
            string FontName = "System";//"Times New Roman";
            Color ColorHeader = Color.FromArgb(200, 200, 200); 
            Color ColorPanel = Color.FromArgb(240, 240, 240);
            //кнопка на портрете героя
            btn_Hero.Size(105, 90); btn_Hero.Location(0.221, 0.021); btn_Hero.Text = "";
            btn_Hero.Button_Styles(Color.Transparent, Color.FromArgb(255, 206, 185, 154), Color.Transparent, Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), 0);
            //кнопка смены фоновой картинки на вкладке "Деревня"
            btn_NextbgVillage.Size(150, 40);
            btn_NextbgVillage.Location(LC.bnbv_X, LC.bnbv_Y);
            btn_NextbgVillage.Button_Styles(Color.FromArgb(100, 150, 100), Color.Red, Color.Transparent, 
                                            Color.FromArgb(255, 255, 0), Color.FromArgb(255, 255, 0), 1);
            //панель ресурсов на складе
            lb_wood.Font = lb_clay.Font = lb_iron.Font = lb_crop.Font = lb_cons_crop.Font = lb_gold.Font = new Font(FontName, FSize, FontStyle.Bold);
            Panel_Resources.BackColor = Color.FromArgb(255, 239, 227, 202);//фон на панели
            Panel_Resources.Contour_Solid(3, Color.FromArgb(255, 165, 145, 125));//рамка на панели
            //панель добычи ресурсов
            lb_Resources_Production_Header.Font = new Font(FontName, FSize + 2, FontStyle.Bold);
            Panel_Resources_Production.Height = lb_Resources_Production_Header.Height + ToCSR(22);
            Panel_Resources_Production.BackColor = ColorPanel;//фон на панели
            //Panel_Resources_Production.Contour_Solid(1, Color.FromArgb(0, 0, 0));//рамка на панели
                //цвет : 2 фона + рамка + цвет шрифта
                grid_Resources_Production.BackgroundColor = grid_Resources_Production.GridColor =
                grid_Resources_Production.DefaultCellStyle.BackColor =
                grid_Resources_Production.Columns[0].DefaultCellStyle.ForeColor = ColorPanel;
                //шрифт
                grid_Resources_Production.Columns[0].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);//pic
                grid_Resources_Production.Columns[1].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);//res
                grid_Resources_Production.Columns[2].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);//value
                grid_Resources_Production.Columns[3].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);//in hour
                //автоматическое изменение ширины столбца чтобы поместился весь текст
                //grid_Resources_Production.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;//авто ширина всех ячеек
                grid_Resources_Production.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Resources_Production.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Resources_Production.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //выравнивание содержимого ячейки относительно верх/низ, лево/право
                grid_Resources_Production.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid_Resources_Production.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //панель информации о деревне
            float SF; if (Global.Screen_Height >= 1080) SF = 10F; else SF = 9F;/*шрифт*/
            lb_Village1.SizeFont(SF, FontStyle.Bold); lb_Village2.SizeFont(SF, FontStyle.Bold); lb_Village3.SizeFont(SF, FontStyle.Bold);
            lb_Village4.SizeFont(SF, FontStyle.Bold); lb_Village5.SizeFont(SF, FontStyle.Bold);
            int tabY2 = ToCSR(6);/*межстрочный отступ*/
            int tab_Top1 = ToCSR(11);/*позиция первой строки*/
            int tab_Top2 = ToCSR(11);/*позиция первой строки*/
            //ноутбук 1366х768
            /*y1080*/if (SCREEN(max:1080)) { tab_Top1 = ToCSR(14); tab_Top2 = ToCSR(22); tabY2 = ToCSR(6);/*+*/ }
            /*y900*/else if (SCREEN(900, 1080)) { tab_Top1 = ToCSR(15); tab_Top2 = ToCSR(21); tabY2 = ToCSR(8);/*+*/ }
            /*y768*/else if (SCREEN(768, 900)) { tab_Top1 = ToCSR(13); tab_Top2 = ToCSR(19); tabY2 = ToCSR(9);/*+*/ }
            /*y720*/else if (SCREEN(min: 768)) { tab_Top1 = ToCSR(11); tab_Top2 = ToCSR(18); tabY2 = ToCSR(10);/*+*/ }
            lb_Village1.Top = tab_Top1; lb_Village2.Top = lb_Village1.Top + lb_Village1.Height; lb_Village3.Top = lb_Village2.Top + lb_Village2.Height;
            lb_Village4.Top = tab_Top2; lb_Village5.Top = lb_Village4.Top + lb_Village4.Height + tabY2;

            //состояние кнопок переключения между вкладками
            pb_Button_Resourses.Tag = 1; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
            pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;
            pb_Button_Resourses.ReSize(); pb_Button_Village.ReSize(); pb_Button_Map.ReSize();
            pb_Button_Statistics.ReSize(); pb_Button_Reports.ReSize(); pb_Button_Messages.ReSize();
            pb_Button_Resourses.Location(0.2980, 0.0012); pb_Button_Village.Location(0.3450, 0.0012);
            pb_Button_Map.Location(0.3955, 0.0012);       pb_Button_Statistics.Location(0.4375, 0.0012);
            pb_Button_Reports.Location(0.4795, 0.0012);   pb_Button_Messages.Location(0.5215, 0.0012);
            //состояние, размеры и положение кнопок изменения скорости игры
            int size = 50;
            btn_SpeedGame1.Tag = btn_SpeedGame3.Tag = btn_SpeedGame4.Tag = btn_SpeedGame5.Tag = btn_SpeedGame6.Tag = 0;
            btn_SpeedGame2.Tag = 1;
            btn_SpeedGame1.Size(size, size); btn_SpeedGame2.Size(size, size);
            btn_SpeedGame3.Size(size, size); btn_SpeedGame4.Size(size, size);
            btn_SpeedGame5.Size(size, size); btn_SpeedGame6.Size(size, size);
            btn_SpeedGame1.Location(0.80, 0.058); btn_SpeedGame2.Location(0.83, 0.058);
            btn_SpeedGame3.Location(0.86, 0.058); btn_SpeedGame4.Location(0.89, 0.058);
            btn_SpeedGame5.Location(0.92, 0.058); btn_SpeedGame6.Location(0.95, 0.058);
            //картинка + текст нации выбранного / созданного аккаунта
            lb_Ethnos.ReSizeFont(FontStyle.Bold); Picture_Ethnos.Location(LC.pe_X, LC.pe_Y); Picture_Ethnos.ReSize();
            lb_Ethnos.Location(LC.le_X, LC.le_Y);
            //интерфейс - Map
                btn_Map_GetterTools.Font = new Font(FontName, FSize, FontStyle.Bold);
                btn_Map_GetterTools.Location(0.61, 0.027);
                btn_Map_GetterTools.Button_Styles(Color.FromArgb(102, 79, 65), Color.FromArgb(255, 0, 0), 
                    Color.FromArgb(0, 0, 0), Color.FromArgb(200, 165, 128), Color.FromArgb(215, 175, 135), 0);
                //панель сверху с координатами в скобках
                txt_Map_Coord.SizeFont(txt_Map_Coord.Font.Size, FontStyle.Bold);
                pnl_Map_Coord.ReSize(); pnl_Map_Coord.Location(0.1620, 0.2150);
                //панель внизу с вводом координат и кнопкой ОК
                tb_Coord_X.BackColor = Color.FromArgb(200, 225, 220); tb_Coord_Y.BackColor = Color.FromArgb(200, 225, 220);
                txt_Map_X.SizeFont(txt_Map_X.Font.Size, FontStyle.Bold);   txt_Map_Y.SizeFont(txt_Map_Y.Font.Size, FontStyle.Bold);
                tb_Coord_X.SizeFont(tb_Coord_X.Font.Size, FontStyle.Bold); tb_Coord_Y.SizeFont(tb_Coord_Y.Font.Size, FontStyle.Bold);
                btn_Map_OK.SizeFont(btn_Map_OK.Font.Size, FontStyle.Bold);
                btn_Map_OK.ReSize(); tb_Coord_X.ReSize(); tb_Coord_Y.ReSize();
                int Top = ToCSR(10); int Left = ToCSR(10); int tab = ToCSR(15);
                /*X:*/txt_Map_X.Location = new Point(Left, 10);
                /*[]*/tb_Coord_X.Location = new Point(txt_Map_X.Left + txt_Map_X.Width, Top);
                /*Y:*/txt_Map_Y.Location = new Point(tb_Coord_X.Left + tb_Coord_X.Width + tab, 10);
                /*[]*/tb_Coord_Y.Location = new Point(txt_Map_Y.Left + txt_Map_Y.Width, Top);
                btn_Map_OK.Location = new Point(tb_Coord_Y.Left + tb_Coord_Y.Width + 5, Top);
                pnl_Map_Input_Coord.Size = new Size(btn_Map_OK.Left + btn_Map_OK.Width + Left, btn_Map_OK.Top + btn_Map_OK.Height + Top);
                pnl_Map_Input_Coord.Location(0.0900, 0.8150);
                //карта
                pb_MAP.Location(LC.pm_X, LC.pm_Y);
                //панель с данными о ячейке
                Right1_Map_DATA.Text = ""; Right2_Map_DATA.Text = ""; Right3_Map_DATA.Text = "";
                pnl_Map_DATA.BackColor = Color.White; pnl_Map_DATA.BorderStyle = BorderStyle.FixedSingle;
                txt_Map_DATA.SizeFont(txt_Map_DATA.Font.Size, FontStyle.Bold);
                Left1_Map_DATA.SizeFont(Left1_Map_DATA.Font.Size, FontStyle.Bold); Left2_Map_DATA.SizeFont(Left2_Map_DATA.Font.Size, FontStyle.Bold);
                Left3_Map_DATA.SizeFont(Left3_Map_DATA.Font.Size, FontStyle.Bold); Right1_Map_DATA.SizeFont(Right1_Map_DATA.Font.Size, FontStyle.Bold);
                Right2_Map_DATA.SizeFont(Right2_Map_DATA.Font.Size, FontStyle.Bold); Right3_Map_DATA.SizeFont(Right3_Map_DATA.Font.Size, FontStyle.Bold);
                Left1_Map_DATA.ReSize(); Left2_Map_DATA.ReSize(); Left3_Map_DATA.ReSize(); Right1_Map_DATA.ReSize(); Right2_Map_DATA.ReSize(); Right3_Map_DATA.ReSize();
                Top = ToCSR(5); Left = ToCSR(0); txt_Map_DATA.Top = Top;
                Left1_Map_DATA.Location = new Point(Left, txt_Map_DATA.Top + txt_Map_DATA.Height + ToCSR(10));
                Left2_Map_DATA.Location = new Point(Left, Left1_Map_DATA.Top + Left1_Map_DATA.Height + 1);
                Left3_Map_DATA.Location = new Point(Left, Left2_Map_DATA.Top + Left2_Map_DATA.Height + 1);
                Right1_Map_DATA.Location = new Point(Left1_Map_DATA.Left + Left1_Map_DATA.Width + 1, Left1_Map_DATA.Top);
                Right2_Map_DATA.Location = new Point(Left2_Map_DATA.Left + Left2_Map_DATA.Width + 1, Left2_Map_DATA.Top);
                Right3_Map_DATA.Location = new Point(Left3_Map_DATA.Left + Left3_Map_DATA.Width + 1, Left3_Map_DATA.Top);
                txt_Map_DATA.Left = (pnl_Map_DATA.Width - txt_Map_DATA.Width) / 2;
                pnl_Map_DATA.Location = new Point(pb_MAP.Left + (int)(pb_MAP.Width * 0.66), pnl_Map_Coord.Top - ToCSR(0));
                //картинка со стрелочками
                pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX.png");
                pb_XXXX_Map.Size(pb_XXXX_Map.BackgroundImage.Width, pb_XXXX_Map.BackgroundImage.Height);
                //изменение размера карты слева вверху над картой
                pnl_Map_Size.BackColor = Color.White;
                nud_value_size_map.SizeFont(nud_value_size_map.Font.Size, FontStyle.Bold);
                lb_text_size_map.SizeFont(lb_text_size_map.Font.Size, FontStyle.Bold);
                nud_value_size_map.Location(0.065, 0.065); lb_text_size_map.Location = new Point(nud_value_size_map.Left + nud_value_size_map.Width, ToCSR(10));
                pnl_Map_Size.Location(0.0, 0.0); 
                //кнопка FULL SCREEN
                btn_Map_Full_Screen.ReSize(); btn_Map_Full_Screen.Location = new Point(ToCSR(15), pnl_Map_Size.Top + pnl_Map_Size.Height + ToCSR(5));
                btn_Map_Full_Screen.Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent,
                    btn_Map_Full_Screen.FlatAppearance.BorderColor, Color.Black, 1);
                //текст на панели подсказки (название деревни + население)
                lb_Map_Info_Village_Population.SizeFont(lb_Map_Info_Village_Population.Font.Size, FontStyle.Bold);
                lb_Map_Info_Village_Population.Location = new Point(10, 10); pnl_Map_Info_Village_Population.Visible = false;
                pnl_Map_Info_Village_Population.BackColor = Color.FromArgb(255, 165, 145, 125);//фон на панели
                //4 кнопки на картинке с зелёными стрелочками
                btn_Map_Left_X.Size(63, 36); btn_Map_Right_X.Size(63, 36); btn_Map_Up_Y.Size(63, 36); btn_Map_Down_Y.Size(63, 36);
                btn_Map_Left_X.Location = new Point(0, 0);
                btn_Map_Right_X.Location = new Point(pb_XXXX_Map.Width - btn_Map_Right_X.Width, pb_XXXX_Map.Height - btn_Map_Right_X.Height);
                btn_Map_Up_Y.Location = new Point(pb_XXXX_Map.Width - btn_Map_Up_Y.Width, 0);
                btn_Map_Down_Y.Location = new Point(0, pb_XXXX_Map.Height - btn_Map_Down_Y.Height);
                btn_Map_Left_X.Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent, Color.Red, Color.Black, 0);
                btn_Map_Right_X.Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent, Color.Red, Color.Black, 0);
                btn_Map_Up_Y.Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent, Color.Red, Color.Black, 0);
                btn_Map_Down_Y.Button_Styles(Color.Transparent, Color.Transparent, Color.Transparent, Color.Red, Color.Black, 0);

            //панель информации о войсках
            lb_Army_Header.Font = new Font(FontName, FSize + 2, FontStyle.Bold);
            Panel_Army.Height = lb_Army_Header.Height + ToCSR(10);
            lb_Army_Header.Centering_Y(ToCSR(10));
            Panel_Army.BackColor = ColorHeader;//фон на панели
            //Panel_Army.Contour_Solid(1, Color.FromArgb(0, 0, 0));//рамка на панели
            //цвет : 2 фона + рамка + цвет шрифта
                grid_Army.BackgroundColor = grid_Army.GridColor = grid_Army.DefaultCellStyle.BackColor =
                grid_Army.Columns[0].DefaultCellStyle.ForeColor = ColorPanel;
                grid_Army.Columns[1].DefaultCellStyle.ForeColor = Color.Blue;//войска
                //шрифт
                //grid_Army.ColumnHeadersDefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);//заголовок
                grid_Army.Columns[1].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);//ячейки
                grid_Army.Columns[2].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);//ячейки
                //автоматическое изменение ширины столбца чтобы поместился весь текст
                grid_Army.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Army.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //выравнивание содержимого ячейки относительно верх/низ, лево/право
                grid_Army.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //скрыть выделение ячеек
                grid_Army.DefaultCellStyle.SelectionBackColor = grid_Army.DefaultCellStyle.BackColor;
                grid_Army.DefaultCellStyle.SelectionForeColor = Color.Black;

            //панель передвижения юнитов (войска, поселенцы, говоруны и т.д.)
            lb_Move_Units_Header.Font = new Font("Arial", FSize + 2, FontStyle.Bold);
            Panel_Move_Units.Height = lb_Move_Units_Header.Height + ToCSR(10);
            lb_Move_Units_Header.Centering_Y(ToCSR(10));
            Panel_Move_Units.BackColor = ColorPanel;//фон на панели
            //Panel_Move_Units.Contour_Solid(3, Color.FromArgb(0, 0, 0));//рамка на панели
            //цвет : 2 фона + рамка + цвет шрифта
                grid_Move_Units.BackgroundColor = grid_Move_Units.GridColor = grid_Move_Units.DefaultCellStyle.BackColor =
                grid_Move_Units.Columns[0].DefaultCellStyle.ForeColor = ColorPanel;
                //шрифт
                grid_Move_Units.Columns[1].DefaultCellStyle.Font = new Font("Arial", FSize, FontStyle.Bold);//ячейки
                grid_Move_Units.Columns[2].DefaultCellStyle.Font = new Font("Arial", FSize, FontStyle.Regular);//ячейки
                //автоматическое изменение ширины столбца чтобы поместился весь текст
                grid_Move_Units.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Move_Units.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //выравнивание содержимого ячейки относительно верх/низ, лево/право
                grid_Move_Units.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid_Move_Units.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                grid_Move_Units.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //панель строительства построек в ресурсных полях и в деревне
            lb_Construction_Header.Font = new Font(FontName, FSize + 2, FontStyle.Bold);
            btn_Construction.Font = new Font(FontName, FSize, FontStyle.Bold);
            btn_Construction.Top = ToCSR(10);//горизонталь вычисляется в методе панели строительства построек
            lb_Construction_Header.Location = new Point(ToCSR(10), btn_Construction.Top + btn_Construction.Height - lb_Construction_Header.Height);
            Panel_Construction.BackColor = ColorPanel;//фон на панели
            Panel_Construction.Contour_Solid(2, Color.FromArgb(0, 0, 0));//рамка на панели

            //вкладка "статистика"
            for (var i = 0; i < grid_Statistics.Columns.Count; i++ ) {
                //текст заголовков в шапке выравниваем по центру
                grid_Statistics.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //текст ячеек выравниваем по центру
                grid_Statistics.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid_Statistics.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            } grid_Statistics.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;  //игрок
            //Font шрифтов
            grid_Statistics.Columns[1].DefaultCellStyle.ForeColor = grid_Statistics.Columns[2].DefaultCellStyle.ForeColor = Color.FromArgb(110, 160, 25);
            grid_Statistics.Columns[1].DefaultCellStyle.Font = grid_Statistics.Columns[2].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);
            //grid_Statistics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;//выравнивание ячеек по ширине всей таблицы
            //по дефолту выбрана вкладка "Игроки"
            btn_Statistics_Players.Tag = 1; btn_Statistics_Alliances.Tag = btn_Statistics_Villages.Tag = btn_Statistics_Heroes.Tag = btn_Statistics_Wonders.Tag = 0;
            tb_rank_Statistics_Footer.AutoSize = tb_name_Statistics_Footer.AutoSize = true;

            //вкладка "отчёты"
            for (var i = 0; i < grid_Reports.Columns.Count; i++ ) {
                grid_Reports.Columns[i].HeaderText = "";
                //текст заголовков в шапке выравниваем по центру
                grid_Reports.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //текст ячеек выравниваем по центру
                grid_Reports.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                /*Font текста*/grid_Reports.Columns[i].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);
                /*Font заголовков*/grid_Reports.Columns[i].HeaderCell.Style.Font = new Font(FontName, FSize, FontStyle.Bold);
            } grid_Reports.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;//события по левому краю
            //Font шрифтов
            grid_Reports.Columns[3].DefaultCellStyle.ForeColor = Color.FromArgb(110, 160, 25);
            grid_Reports.Columns[5].DefaultCellStyle.ForeColor = Color.Black;
            grid_Reports.Columns[5].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);
            //заполнение колонками всей ширины таблицы
            grid_Reports.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //по дефолту выбрана вкладка "Все"
            btn_Reports_All.Tag = 1; btn_Reports_Army.Tag = btn_Reports_Trading.Tag = btn_Reports_Other.Tag = btn_Reports_Archive.Tag = btn_Reports_Neighborhood.Tag = 0;

            //вкладка "Сообщения"
            for (var i = 0; i < grid_Messages.Columns.Count; i++ ) {
                grid_Messages.Columns[i].HeaderText = "";
                grid_Messages.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid_Messages.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                /*Font текста*/grid_Messages.Columns[i].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Bold);
                /*Font заголовков*/grid_Messages.Columns[i].HeaderCell.Style.Font = new Font(FontName, FSize, FontStyle.Bold);
                grid_Messages.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            } grid_Messages.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;//темы по левому краю
            grid_Messages.Columns[2].DefaultCellStyle.ForeColor = Color.FromArgb(110, 160, 25);
            grid_Messages.Columns[4].DefaultCellStyle.ForeColor = Color.Black;
            grid_Messages.Columns[4].DefaultCellStyle.Font = new Font(FontName, FSize, FontStyle.Regular);
            grid_Messages.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //по дефолту выбрана вкладка "Входящие"
            btn_Messages_Incoming.Tag = 1; btn_Messages_Outgoing.Tag = btn_Messages_Archive.Tag = btn_Messages_Write.Tag = 0;
        }

        public void Illumination(int left, int top, int width, int height, Color MIN, Color MAX, double Alpha, byte mod, byte num) {
            if (Global.Screen_Width < 1920) return;//подсветка глючит с другими разрешениями экрана. надо допиливать
            if (Selected_Cell != null) Selected_Cell.Dispose(); byte _num = num;
            Bitmap bmp = new Bitmap(ToCSR(width), ToCSR(height)); Bitmap BMP = Extensions.getBitmap(bg_Resources);
            for (int y = 0; y < bmp.Height; y++) for (int x = 0; x < bmp.Width; x++) {
                Color cl = BMP.GetPixel(left + x, top + y); 
                if (num == _num) {
                    if (cl.R >= MIN.R && cl.G >= MIN.G && cl.B >= MIN.B &&
                        cl.R <= MAX.R && cl.G <= MAX.G && cl.B <= MAX.B) { 
                        int R = cl.R, G = cl.G, B = cl.B;
                        int gray = (cl.R + cl.G + cl.B) / 3;
                        int LAB = ((int)(R * 0.299 + G * 0.587 + B * 0.114));
                        int Const = (gray + LAB) * 2; UFO.Convert.toRGB(ref Const);
                        //Alpha: 0 - прозрачный, 1 - не прозрачный 100%
                        switch (mod) { 
                            case 0: 
                                R = (int)(Alpha * Const + (1 - Alpha) * cl.R);
                                G = (int)(Alpha * Const + (1 - Alpha) * cl.G);
                                B = (int)(Alpha * Const + (1 - Alpha) * cl.B);
                            break;
                            case 1: R = G = B = (int)(Alpha * Const + (1 - Alpha) * cl.R); break;//gray
                            case 2: R = (int)(Alpha * Const + (1 - Alpha) * cl.R); G = B = 0; break;//red
                        }
                        toRGB(ref R, ref G, ref B); cl = Color.FromArgb(255, R, G, B);
                    }
                } if (num > 0) num--; else num = _num; bmp.SetPixel(x, y, cl);
            }
            Selected_Cell = new PictureBox() {
                Width = bmp.Width, Height = bmp.Height, Left = ToCSR(left), Top = ToCSR(top),
                BackgroundImage = bmp, Parent = bg_Resources,
            }; 
            //Selected_Cell.ReSize();
        }

        /// <summary> Метод меняет фоновую картинку на вкладке "РЕСУРСНЫЕ ПОЛЯ" в зависимости от типа выбранной деревни. </summary>
        /// <remarks>
        ///     Внимание! <br/>
        ///     Чтобы добавить новый тип ресурсного поля, нужно выполнить следующее: <br/>
        ///     - Внести изменения в этом методе. <br/>
        ///     - Подготовить картинку и сохранить её в папке <b>"DATA_BASE/IMG/BackGround/".</b> <br/>
        ///     - Подготовить картинку и сохранить её в папке: <b>"DATA_BASE/IMG/map/Fields/".</b> <br/>
        ///     - Добавить информацию в <b>enum: TypeCell</b> <br/>
        ///     - Внести изменение в методе: <b>TMap.CreateMap(...);</b> <br/>
        ///     - Внести изменение в методе: <b>winDlg_InfoCell</b> в файле Form1_DialogWindows.cs <br/>
        ///     - Создать json файл <b>Default_Village_slots-?-?-?-?</b> в папке: <b>DATA_BASE/JSON</b> <br/>
        /// </remarks>
        public void BackGroundResources() {
            string path = "DATA_BASE/IMG/BackGround/resource_1503x829-";
            switch (Player.VillageList[Player.ActiveIndex].Type_Resources) {
                case TypeCell._4_4_4_6: path += "4-4-4-6.png"; break;  case TypeCell._4_4_3_7: path += "4-4-3-7.png"; break;
                case TypeCell._4_3_5_6: path += "4-3-5-6.png"; break;  case TypeCell._3_4_4_7: path += "3-4-4-7.png"; break;  
                case TypeCell._1_1_1_15: path += "1-1-1-15.png"; break;
                default: path += "4-4-4-6.png"; break;
            }
            //path = "DATA_BASE/IMG/BackGround/resource_1503x829-3-4-4-7.png";//тест новой картинки чтобы провирить
            bg_Resources.BackgroundImage = Image.FromFile(path);//лепим соответствующую картинку
        }

        /// <summary> Метод получает картинку (пиктограмму/иконку) по указанному пути и масштабирует её в заданный размер. </summary>
        /// <remarks> Корректный путь <b>path + FileName</b> = "MAIN_FOLDER/folder1/folder2<b>/</b>" + "picture.bmp"; </remarks>
        /// <value> <b> <paramref name="path"/>: </b> путь к файлу. <br/> <b> <paramref name="FileName"/>: </b> имя файла. <br/> <b> <paramref name="w"/>/<paramref name="h"/>: </b> желаемая ширина и высота картинки. <br/> </value>
        /// <returns> Возвращает картинку. <br/> Если картинка не найдена, метод пытается вернуть изображение "null.png", лежащее в той же папке; <br/> Если и его нет, то возвращает <b>null</b>. </returns>
        private Image GetICO(string path, string FileName, int w, int h) {
            return Extensions.ResizeImage(File.Exists($"{path}{FileName}") ? Image.FromFile($"{path}{FileName}")
                : File.Exists($"{path}/null.png") ? Image.FromFile($"{path}/null.png") : null, ToCSR(w), ToCSR(h));
        }


        /// <summary> Вспомогательный метод к методу <b>Get_ComboBox_Items()</b>. Ищет в слотах деревни постройку <b>Search</b> уровня <b>lvl</b> или выше. </summary>
        /// <returns> Возвращает <b>true</b> если в деревне есть искомая постройка <b>Search</b> уровня <b>lvl</b> или выше, в противном случае возвращает <b>false</b>. </returns>
        private bool Search(Buildings Search, int lvl = -1) {
            bool b = false; /*default :: для одиночных построек. true = в деревне есть искомая постройка уровня lvl*/
            int min, max; if ((int)Search >= 1 && (int)Search <= 4) { min = 0; max = 18; }//ресурсы
            else if (Search == Buildings.Главное_здание) { min = 27; max = 28; }//27 ГЗ
            else if (Search == Buildings.Пункт_сбора) { min = 19; max = 20; }//19 Пункт сбора
            else if ((int)Search >= 31 && (int)Search <= 33) { min = 18; max = 19; }//18 стены
            else { min = 18; max = 40; }
            for (int v = min; v < max; v++) { var Slot = Player.VillageList[Player.ActiveIndex].Slot[v];
                if (Slot.ID == Search && Slot.Level >= lvl) { return true; }//нашли среди построенных
            }
            //ищем среди строящихся
            var Коры = Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian; var p = GAME.Map.Coord_WorldToMap(Коры);
            var Stack = GAME.Event_Stack.Stack;
            for (int i = 0; i < Stack.Count; i++) if (Stack[i].TypeEvent == Type_Event.Construction)
                if (Stack[i].Cell_Start == p) if (Stack[i].ID == Search && Stack[i].lvl >= lvl) { return true; }
            return b;
        }
        /// <summary> Метод парсит строку <b>Text</b>, что является строкой вида: "12. Госпиталь", или "36. Госпиталь" из коллекции <b>ComboBox.Items</b> выпадающего списка и получает номер слота или <b>ID</b> постройки, в зависимости от того каким методом создавали эти строки. </summary>
        /// <returns> Возвращает номер слота или номер <b>ID</b> постройки, в случае неудачи возвращает <b>-1.</b> </returns>
        private int Get_Number_From_ComboBox_Items(string Text) { string tmp = "";
            for (int i = 0; i < Text.Length; i++) if (Text[i] >= 0 + 48 && Text[i] <= 9 + 48) tmp += Text[i]; else break;
            if (tmp != "") return System.Convert.ToInt32(tmp); else return -1;
        }

        /// <summary>
        ///     Метод формирует список построек которые можно снести в главном здании. <br/> В список не попадают: ресурсные поля, главное здание, пункт сбора, стены и ЧУДО. <br/>
        ///     ЭТОТ СПИСОК АКТУАЛЕН ТОЛЬКО ДЛЯ ВКЛАДКИ "ДЕРЕВНЯ". <br/> 
        ///     Число вначале строки соответствует номеру слота. <br/>
        ///     Чтобы получить <b> NumberSlot </b> постройки по названию строки в впадающем списке,
        ///     следует передать строку в метод <b> Get_ID_From_ComboBox_Items(...) </b>. 
        /// </summary>
        /// <returns> Возвращает массив <b> string[]</b> с названиями разрешённых построек в активной деревне формата: string[N] = "18. Стена", или string[N] = "40. Водопой". </returns>
        private string[] Get_ComboBox_Items_Destruction() {
            List<string> Builds = new List<string>();
            var Slot = Player.VillageList[Player.ActiveIndex].Slot;
            for (int i = 18; i < Slot.Length; i++) if (Slot[i].Level > 0) 
                if (Slot[i].ID != Buildings.ЧУДО_СВЕТА && Slot[i].ID != Buildings.Главное_здание &&
                    Slot[i].ID != Buildings.Пункт_сбора && Slot[i].ID != Buildings.Городская_стена_Римляне &&
                    Slot[i].ID != Buildings.Земляной_вал_Германцы && Slot[i].ID != Buildings.Изгородь_Галлы &&
                    Slot[i].ID != Buildings.Стена_Натары)
                    Builds.Add(i + ". " + LANGUAGES.buildings[(int)Slot[i].ID] + " " + LANGUAGES.RESOURSES[112]);//добавляем строку в лист
            return Builds.ToArray();
        }

        /// <summary> 
        ///     Метод формирует список разрешённых построек на данном уровне развития активной деревни. <br/> 
        ///     Некоторые деревни требуют наличие других построек определённого уровня. <br/> 
        ///     ЭТОТ СПИСОК АКТУАЛЕН ТОЛЬКО ДЛЯ ВКЛАДКИ "ДЕРЕВНЯ". <br/> 
        ///     Число вначале строки соответствует <b>ID</b> слота. <br/>
        ///     Чтобы получить <b> ID </b> постройки по названию строки в впадающем списке,
        ///     следует передать строку в метод <b> Get_ID_From_ComboBox_Items(...) </b>. 
        /// </summary>
        /// <remarks> 
        ///     Параметр <b> Add </b> служит для того чтобы возможно было выписать в лист все постройки какие есть для тестов. <br/> 
        ///     При <b> Add </b> = false, в ComboBox лист загрузятся постройки которые удовлетворяют требованиям. <br/>
        ///     При <b> Add </b> = true, в ComboBox лист загрузятся все постройки какие есть.
        /// </remarks>
        /// <returns> Возвращает массив <b> string[]</b> с названиями разрешённых построек в активной деревне формата: string[N] = "4. Ферма", или string[N] = "23. Тайник". </returns>
        private string[] Get_ComboBox_Items(bool Add = false) {
            List<string> Builds = new List<string>();
            var Village = Player.VillageList[Player.ActiveIndex]; var Slot = Village.Slot;
            for (int i = 0; i < GAME.Build.Length; i++) {
                bool add = false;/*default :: по дефолту добавлять постройку в список нельзя*/
                //добавляем в лист все постройки которые удовлетворяют требованиям
                switch (i) {
                    case (int)Buildings.ЧУДО_СВЕТА: //требования: деревня Натаров
                        if (Village.Type_Village == TypeVillage.Natars)//натарская деревня
                            if (Slot[19].ID == Buildings.ПУСТО) add = true;//слот с чудом пустой
                        break;
                    case 1: case 2: case 3: case 4: break;//пропускаем ресурсные постройки

                    //add = !Search (инверсия) - ОЗНАЧАЕТ ОТСУТСТВИЕ, МОЖНО СТРОИТЬ ТОЛЬКО ОДНО ТАКОЕ СТРОЕНИЕ В ДЕРЕВНЕ
                    //add = true/false - ОЗНАЧАЕТ ДОБАВЛЯЕМ ИЛИ НЕ ДОБАВЛЯЕМ СТРОКУ В ЛИСТ ПОСТРОЕК
                    //Search(...) - ОЗНАЧАЕТ НАЛИЧИЕ ЗДАНИЯ, ЕДИНИЧНОЕ ТРЕБОВАНИЕ КОТОРЫХ МОЖЕТ БЫТЬ НЕСКОЛЬКО У ПОСТРОЙКИ

                    case (int)Buildings.Лесопильный_завод: 
                        if (Search(Buildings.Лесопилка, 10)) if (Search(Buildings.Главное_здание, 5)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Кирпичный_завод: 
                        if (Search(Buildings.Глиняный_карьер, 10)) if (Search(Buildings.Главное_здание, 5)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Чугунолитейный_завод: 
                        if (Search(Buildings.Железный_рудник, 10)) if (Search(Buildings.Главное_здание, 5)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Мукомольная_мельница: 
                        if (Search(Buildings.Ферма, 5)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Пекарня:
                        if (Search(Buildings.Главное_здание, 5)) if (Search(Buildings.Ферма, 10)) 
                            if (Search(Buildings.Мукомольная_мельница, 5)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Склад:
                        if (Search((Buildings)i, 20)) add = true; else add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Амбар: 
                            if (Search((Buildings)i, 20)) add = true; else add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Госпиталь:
                        if (Search(Buildings.Главное_здание, 10)) if (Search(Buildings.Академия, 15)) 
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Кузница:
                        if (Search(Buildings.Главное_здание, 3)) if (Search(Buildings.Академия, 1)) 
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Арена: if (Search(Buildings.Пункт_сбора, 15)) add = true; break;
                    case (int)Buildings.Главное_здание: add = false; break;//требований нет
                    case (int)Buildings.Пункт_сбора: add = false; break;//требований нет
                    case (int)Buildings.Рынок:
                        if (Search(Buildings.Главное_здание, 3)) if (Search(Buildings.Склад, 1)) 
                            if (Search(Buildings.Амбар, 1)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Посольство: add = !Search((Buildings)i); break;
                    case (int)Buildings.Казарма:
                        if (Search(Buildings.Главное_здание, 3)) if (Search(Buildings.Пункт_сбора, 1))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Конюшня:
                        if (Search(Buildings.Кузница, 3)) if (Search(Buildings.Академия, 5))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Мастерская:
                        if (Search(Buildings.Главное_здание, 5)) if (Search(Buildings.Академия, 10))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Академия:
                        if (Search(Buildings.Главное_здание, 3)) if (Search(Buildings.Казарма, 3))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Тайник: add = true; break;//требований нет
                    case (int)Buildings.Ратуша:
                        if (Search(Buildings.Главное_здание, 10)) if (Search(Buildings.Академия, 10))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Резиденция:
                        if (Search(Buildings.Главное_здание, 5)) if (!Search(Buildings.Дворец))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Дворец:
                        if (Search(Buildings.Главное_здание, 5)) if (Search(Buildings.Посольство, 1))
                            if (!Search(Buildings.Резиденция)) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Сокровищница:
                        if (Search((Buildings)i, 20)) add = true; else add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Торговая_палата:
                        if (Search(Buildings.Рынок, 20)) if (Search(Buildings.Конюшня, 10))
                                add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Большая_казарма: if (Search(Buildings.Казарма, 20)) add = !Search((Buildings)i); break;
                    case (int)Buildings.Большая_конюшня: if (Search(Buildings.Конюшня, 20)) add = !Search((Buildings)i); break;
                    case (int)Buildings.Городская_стена_Римляне: add = false; break;//требования: Rome (здесь не добавляем)
                    case (int)Buildings.Земляной_вал_Германцы: add = false; break;//требований: German (здесь не добавляем)
                    case (int)Buildings.Изгородь_Галлы: add = false; break;//требований: Gaul (здесь не добавляем)
                    case (int)Buildings.Стена_Натары: add = false; break;//требования: Natar (натарская деревня)
                    case (int)Buildings.Каменотёс:
                        if (Search(Buildings.Главное_здание, 5)) if (Search(Buildings.Дворец, 1))
                            if (Player.NumberOfCapital == Player.ActiveIndex) add = !Search((Buildings)i); 
                        break;
                    case (int)Buildings.Пивоварня_Германцы:
                        if (Search(Buildings.Амбар, 20)) if (Search(Buildings.Пункт_сбора, 10))
                            if (Player.NumberOfCapital == Player.ActiveIndex) 
                                if (Player.Folk_Name == Folk.German) add = !Search((Buildings)i); 
                        break;
                    case (int)Buildings.Капканщик_Галлы:
                        if (Search(Buildings.Пункт_сбора, 1)) if (Player.Folk_Name == Folk.Gaul) add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Таверна:
                        if (Search(Buildings.Главное_здание, 3)) if (Search(Buildings.Пункт_сбора, 1))
                            add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Большой_склад:
                        if (Search(Buildings.Главное_здание, 10)) if (Search(Buildings.Склад, 20))
                            if (Search((Buildings)i, 20)) add = true; else add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Большой_амбар:
                        if (Search(Buildings.Главное_здание, 10)) if (Search(Buildings.Амбар, 20))
                                if (Search((Buildings)i, 20)) add = true; else add = !Search((Buildings)i);
                        break;
                    case (int)Buildings.Водопой_Римляне:
                        if (Search(Buildings.Конюшня, 20)) if (Search(Buildings.Пункт_сбора, 10))
                            if (Player.Folk_Name == Folk.Rome) add = !Search((Buildings)i);
                        break;
                }
                //проверить позиции []<> в rtf файлах
                //для каждого языка
                //add = true;//тест всех построек
                if (Add) add = Add;//если Add = true, тогда забиваем в лист все постройки какие есть для тестов
                if (add) Builds.Add(i + ". " + LANGUAGES.buildings[i]);//добавляем строку в лист
            }
            return Builds.ToArray();
        }

        //========================================================================================================================
        //                                           КОНЕЦ БЛОКА: ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        //========================================================================================================================

        public void tm_Animate_MainMenu_Tick(object sender, EventArgs e) {
            if (Global.timer1_param == 0) { //DOWN
                rich_info.Height += 20; if (rich_info.Height >= Height - rich_info.Top - 153)
                { rich_info.Focus(); tm_Animate_MainMenu.Stop(); }
            } else if (Global.timer1_param == 1) { //UP
                rich_info.Height -= 250; if (rich_info.Height <= 1) { rich_info.Visible = false; tm_Animate_MainMenu.Stop(); }
            }
        }

        private int Panel_Army_Top_prew = 0;
        private int Panel_Village_Top_prew = 0;
        public void tm_Animate_Panel_Army_Tick(object sender, EventArgs e) {
            if (Panel_Army_Top_prew != Panel_Army.Top) {
                //прикрепляем панель с войсками к её хэдеру
                grid_Army.Location = new Point(Panel_Army.Left, Panel_Army.Top + Panel_Army.Height);
                //динамическое изменение высоты панели с войсками в зависимости от её свойства Top
                int gridHeight = grid_Army.ColumnHeadersHeight + grid_Army.Rows.Count * grid_Army.Rows[0].Height + ToCSR(10);
                if (gridHeight <= grid_Army.Parent.Height - grid_Army.Top - ToCSR(20)) grid_Army.Height = gridHeight - ToCSR(20);
                else grid_Army.Height = grid_Army.Parent.Height - grid_Army.Top - ToCSR(20);
            }
            if (Panel_Village_Top_prew != GroupBox_Village.Top) {
                //динамическое изменение высоты панели в зависимости от её свойства Top
                ListBox1.Size = ListBox1.GetPreferredSize(ListBox1.Size); ListBox2.Size = ListBox2.GetPreferredSize(ListBox2.Size);
                int Height = ListBox1.Top + ListBox1.Height + ToCSR(10);
                if (Height <= GroupBox_Village.Parent.Height - GroupBox_Village.Top - ToCSR(20)) GroupBox_Village.Height = Height;
                else GroupBox_Village.Height = GroupBox_Village.Parent.Height - GroupBox_Village.Top - ToCSR(20);
                ListBox1.Height = ListBox2.Height = GroupBox_Village.Height - ListBox1.Top - ToCSR(10);
            }
            Panel_Army_Top_prew = grid_Army.Top;
            Panel_Village_Top_prew = GroupBox_Village.Top;
        }

        public void tabControl_SelectedIndexChanged(object sender, EventArgs e) { TabControl_SelectedIndexChanged(); }
        public void TabControl_SelectedIndexChanged() {
            if (Player.FolderAccount == "") return;
            //блок таймеров для циклического взаимодействия с фронтендом и бэкендом
            if (tabControl.SelectedIndex <= 0) { //в главном меню ставим на ПАУЗУ таймеры
                Event_Timer_Frontend.Stop();
                Event_Thread_Timer_Backend.Change(0, Timeout.Infinite);
            } else {
                if (GAME.SpeedGame <= 0) Event_Timer_Frontend.Interval = 1000; else Event_Timer_Frontend.Interval = GAME.SpeedGame; 
                Event_Timer_Frontend.Start();//продолжаем работу вечного цикла - обработчик событий
                Event_Thread_Timer_Backend.Change(0, GAME.SpeedGame);//без перезапуска таймера
            }

            switch (tabControl.SelectedIndex) {
                default: case 0: //вкладка 0. главное меню
                    //грузим случайный фон
                    bg_Menu.BackgroundImage = LoadRandom_BackgroundImage();
                    statusStrip.Items[0].Text = statusStrip.Items[1].Text = "";
                return;
                case 1: //вкладка 1. ресурсные поля
                    Panel_Village1.Location(LC.pv1_X, LC.pv1_Y);//панель информации о деревни 1
                    Panel_Village2.Location(LC.pv2_X, LC.pv2_Y);//панель информации о деревни 2
                    Panel_Resources_Production.Location(LC.prp_X, LC.prp_Y);//панель добычи ресурсов
                    Panel_Move_Units.Location(LC.pmu_X, LC.pmu_Y);//панель передвижения юнитов
                    GroupBox_Village.Location(LC.gb_X, LC.gb_Y);//панель списка деревень (GroupBox)
                    Panel_Army.Location(LC.pa_X, LC.pa_Y);//панель списка армий
                    Panel_Construction.Location(LC.pc_X, LC.pc_Y);//панель строительства построек
                break;
                case 2: { //вкладка 2. деревня с постройками
                    int tabX = ToCSR(20);//отступ от края и другого объекта
                    //панель добычи ресурсов
                    Panel_Resources_Production.Location = new Point(Global.MainWindowWidth - Panel_Resources_Production.Width - tabX, ToCSR(140));
                    //панель списка деревень
                    GroupBox_Village.Location = new Point(Global.MainWindowWidth - GroupBox_Village.Width - tabX, grid_Resources_Production.Top + grid_Resources_Production.Height + tabX / 2);
                    //панель информации о деревни 1, панель информации о деревни 2
                    Panel_Village1.Location = new Point(tabX, ToCSR(190));
                    Panel_Village2.Location = new Point(tabX, Panel_Village1.Top + Panel_Village1.Height + tabX);
                    //панель строительства построек
                    Panel_Construction.Location = new Point(ToCSR(50), Panel_Construction.Parent.Height - Panel_Construction.Height - ToCSR(50));
                } break;
                case 3: { //вкладка 3. карта
                    int tabX2 = ToCSR(20);//отступ от края и другого объекта
                    //панель списка деревень
                    GroupBox_Village.Location = new Point(GroupBox_Village.Parent.Width - GroupBox_Village.Width - tabX2, ToCSR(150));
                    GroupBox_Village.BringToFront();
                    //панель информации о деревни 1, панель информации о деревни 2
                    Panel_Village1.Location = new Point(tabX2, ToCSR(190));
                    Panel_Village2.Location = new Point(tabX2, Panel_Village1.Top + Panel_Village1.Height + tabX2);
                } break;
                case 4: //вкладка 4. статистика
                    tb_name_Statistics_Footer.Text = Player.Nick_Name;
                    tb_rank_Statistics_Footer.Text = Player.Rank.ToString();
                    break;
                case 5: //вкладка 5. отчёты
                    int tabX3 = ToCSR(10);//отступ от края и другого объекта
                    //панель списка деревень
                    GroupBox_Village.Location = new Point(pnl_Reports_Head.Left + pnl_Reports_Head.Width + tabX3, ToCSR(150));
                    GroupBox_Village.BringToFront();
                    break;
                case 6: //вкладка 6. сообщения
                    int tabX4 = ToCSR(10);//отступ от края и другого объекта
                    //панель списка деревень
                    GroupBox_Village.Location = new Point(pnl_Reports_Head.Left + pnl_Reports_Head.Width + tabX4, ToCSR(150));
                    GroupBox_Village.BringToFront();
                    break;
            }
            _Update();
            //Информационная панель внизу главной формы :: statusStrip
            statusStrip.Items[0].Text = LANGUAGES.RESOURSES[20] + " " + LANGUAGES.RESOURSES[21 + (int)Player.Folk_Name];//Народ: Имя народа
            statusStrip.Items[1].Text = LANGUAGES.RESOURSES[24] + " " + Player.Nick_Name;//Игровой ник: Имя аккаунта
            //запуск/остановка таймера анимации обновления размера панели с войсками в зависимости от положения на форме
            if (tabControl.SelectedIndex == 1 || tabControl.SelectedIndex == 2) {
                tm_Animate_Panel_Army.Interval = 1; tm_Animate_Panel_Army.Enabled = true; }
            else { tm_Animate_Panel_Army.Enabled = false; }
        }
        /// <summary> <inheritdoc cref="IForm1.onPaint"/> </summary>
        /// <remarks> <inheritdoc cref="IForm1.onPaint"/> <br/> <br/> Если не понятно что значат переменные пунктира, открой CustomControls.cs => DottedLine(...); </remarks>
        public void onPaint(object sender, PaintEventArgs e) {
            //пунктирные линии
            if (sender is Control ctrl) {
                float ts = 2; Pen pen = new Pen(Color.FromArgb(100, 100, 100), ts) { DashPattern = new float[] { 4, 2 }, };
                if (ctrl.Name == "Panel_Move_Units") { //панель передвижения войск
                    e.Graphics.DrawRectangle(pen, ts * 0.5f, ts * 0.5f, ctrl.Width - ts, ctrl.Height);
                } else if (ctrl.Name == "Panel_Resources_Production") { //панель производства ресурсов
                    e.Graphics.DrawRectangle(pen, ts * 0.5f, ts * 0.5f, ctrl.Width - ts, ctrl.Height);
                } else if (ctrl.Name == "grid_Move_Units") { //таблица передвижения войск
                    e.Graphics.DrawRectangle(pen, ts * 0.5f, -ts, ctrl.Width - ts, ctrl.Height + ts * 0.5f);
                } else if (ctrl.Name == "grid_Resources_Production") { //таблица производства ресурсов
                    e.Graphics.DrawRectangle(pen, ts * 0.5f, -ts, ctrl.Width - ts, ctrl.Height + ts * 0.5f);
                } else if (ctrl.Name == "btn_Hero") { //кнопка на портрете героя
                    //CustomControls.DottedLine(e, Color.Black, 2, 4, 4, ctrl);
                }
            }
        }

        public void Control_DoubleClick(object sender, EventArgs e) {
            if (sender is PictureBox pb) {
                if (pb.Name == "Panel_Army") pb.Location(LC.pa_X, LC.pa_Y);//панель списка армий
            }
            else if (sender is Panel pnl) {
                if (pnl.Name == "Panel_Move_Units") Panel_Move_Units.Location(LC.pmu_X, LC.pmu_Y);//панель передвижения юнитов
                else if (pnl.Name == "Panel_Resources_Production") { //панель добычи ресурсов
                    if (tabControl.SelectedIndex == 1) pnl.Location(LC.prp_X, LC.prp_Y);
                    else if (tabControl.SelectedIndex == 2) pnl.Location = new Point(Global.MainWindowWidth - pnl.Width - ToCSR(20), ToCSR(50));
                } else if (pnl.Name == "Panel_Construction") { //панель строительства построек
                    if (tabControl.SelectedIndex == 1) pnl.Location(LC.pc_X, LC.pc_Y);
                    else if (tabControl.SelectedIndex == 2) pnl.Location = new Point(ToCSR(50), pnl.Parent.Height - pnl.Height - ToCSR(50));
                }
            } else if (sender is GroupBox gb) { 
                if (gb.Name == "GroupBox_Village") { //панель списка деревень (GroupBox)
                    if (tabControl.SelectedIndex == 1) gb.Location(LC.gb_X, LC.gb_Y);
                    else if (tabControl.SelectedIndex == 2) gb.Location = new Point(Global.MainWindowWidth - gb.Width - ToCSR(20), Panel_Resources_Production.Top + Panel_Resources_Production.Height + ToCSR(20));
                }
            }
        }

        public void Control_Click(object sender, EventArgs e) {
            if (sender is Button btn) {
                //ГЛАВНОЕ МЕНЮ::открыть окошко с выбором ника и созданием игрового мира
                if (btn.Name == "btn_Rome" || btn.Name == "btn_Gauls" || btn.Name == "btn_Teutons") {
                    if (btn.Name == "btn_Rome") form2.Folk_Name = Folk.Rome;
                    if (btn.Name == "btn_Gauls") form2.Folk_Name = Folk.Gaul;
                    if (btn.Name == "btn_Teutons") form2.Folk_Name = Folk.German;
                    form2.ss_NationName.Items[0].Text = LANGUAGES.RESOURSES[21 + (int)form2.Folk_Name]/*Имя народа*/
                        + " :: " + form2.InputName.Text;
                    form2.btn_Start.Enabled = true; form2.ShowDialog(this);
                }
                //кнопки изменения скорости игры
                else if (btn.Name == "btn_SpeedGame1" || btn.Name == "btn_SpeedGame2" || btn.Name == "btn_SpeedGame3" ||
                         btn.Name == "btn_SpeedGame4" || btn.Name == "btn_SpeedGame5" || btn.Name == "btn_SpeedGame6") {
                    btn_SpeedGame1.Tag = btn_SpeedGame2.Tag = btn_SpeedGame3.Tag = btn_SpeedGame4.Tag =
                    btn_SpeedGame5.Tag = btn_SpeedGame6.Tag = 0;
                    btn.Tag = 1;//помечаем активную кнопку
                    btn_SpeedGame1.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame1_0.png");
                    btn_SpeedGame2.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame2_0.png");
                    btn_SpeedGame3.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame3_0.png");
                    btn_SpeedGame4.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame4_0.png");
                    btn_SpeedGame5.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame5_0.png");
                    btn_SpeedGame6.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/btn_SpeedGame6_0.png");
                    btn.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/" + btn.Name + "_1.png");
                    switch (btn.Name) {
                        case "btn_SpeedGame1": GAME.SpeedGame=0;/*PAUSE*/break;      case "btn_SpeedGame2": GAME.SpeedGame=1000;/*PLAY*/break;
                        case "btn_SpeedGame3": GAME.SpeedGame=500;/*SPEED x2*/break; case "btn_SpeedGame4": GAME.SpeedGame=333;/*SPEED x3*/break;
                        case "btn_SpeedGame5": GAME.SpeedGame=200;/*SPEED x5*/break; case "btn_SpeedGame6": GAME.SpeedGame=1;/*SPEED x1000*/break;
                    }
                    if (GAME.SpeedGame <= 0) { Event_Timer_Frontend.Stop(); Event_Thread_Timer_Backend.Change(0, Timeout.Infinite); }
                    else { Event_Timer_Frontend.Interval = GAME.SpeedGame; Event_Timer_Frontend.Start(); Event_Thread_Timer_Backend.Change(0, GAME.SpeedGame); }
                }
                //ВКЛАДКА "РЕСУРСНЫЕ ПОЛЯ"::кнопки запуска построек
                else if(btn.Name == "Button_Slot_resources") {
                    btn.Button_Styles(Color.Transparent, btn.FlatAppearance.MouseDownBackColor, Color.Transparent,
                                                              btn.FlatAppearance.BorderColor, btn.ForeColor, 2);
                    winDlg_LevelUp_Builds((int)btn.Tag);//открываем диалоговое окно и передаём в него номер слота
                }
                //ВКЛАДКА "ДЕРЕВНЯ"::кнопки запуска построек
                else if (btn.Name == "Button_Slot_builds") {
                    if (Build_List != null) {
                        if ((int)btn.Tag != 27) { //любая постройка кроме ГЗ
                            //обновляем список построек
                            Build_List.Items.Clear(); var _Items = Get_ComboBox_Items();
                            if (_Items != null) Build_List.Items.AddRange(_Items);
                            This_List = What_In_Items.Строительство;
                        }
                        else if ((int)btn.Tag == 27) { //главное здание
                            //обновляем список сноса построек
                            Build_List.Items.Clear(); var _Items = Get_ComboBox_Items_Destruction();
                            if (_Items != null) Build_List.Items.AddRange(_Items);
                            This_List = What_In_Items.Снос;
                        }
                        Build_List.Set_SelectedIndex_And_Changed_Ignore(0);//если список пуст, останется -1 без вызова Changed
                    }
                    winDlg_LevelUp_Builds((int)btn.Tag);//открываем диалоговое окно и передаём в него номер слота
                } 
                //кнопка смены фоновой картинки на вкладке "Деревня"
                else if (btn.Name == "btn_NextbgVillage") {
                    GAME.bgVillage_NumberImage++;
                    int bgCount = 0; string[] path = Directory.GetFiles("DATA_BASE/IMG/BackGround/");
                    for (int i = 0; i < path.Length; i++) if (path[i].Contains("Village_1503x829_")) bgCount++;
                    if (GAME.bgVillage_NumberImage < 1) GAME.bgVillage_NumberImage = 1; 
                    else if (GAME.bgVillage_NumberImage > bgCount) GAME.bgVillage_NumberImage = 1;
                    bg_Village.BackgroundImage = Image.FromFile("DATA_BASE/IMG/BackGround/Village_1503x829_" +
                        GAME.bgVillage_NumberImage + ".png");
                }
                //вкладка "СТАТИСТИКА"
                else if (btn.Name == "btn_Statistics_Players" || btn.Name == "btn_Statistics_Alliances" ||
                         btn.Name == "btn_Statistics_Villages" || btn.Name == "btn_Statistics_Heroes" ||
                         btn.Name == "btn_Statistics_Wonders") {
                    btn_Statistics_Players.Tag = btn_Statistics_Alliances.Tag = btn_Statistics_Villages.Tag = btn_Statistics_Heroes.Tag = btn_Statistics_Wonders.Tag = 0;
                    btn.Tag = 1;//отмечаем нажатую кнопку
                    btn_Statistics_Players.BackColor = btn_Statistics_Alliances.BackColor = btn_Statistics_Villages.BackColor =
                    btn_Statistics_Heroes.BackColor = btn_Statistics_Wonders.BackColor = Color.DarkGray;
                    btn_Statistics_Players.ForeColor = btn_Statistics_Alliances.ForeColor = btn_Statistics_Villages.ForeColor =
                    btn_Statistics_Heroes.ForeColor = btn_Statistics_Wonders.ForeColor = Color.Black;
                    btn.BackColor = Color.Black; btn.ForeColor = Color.Transparent; 
                    _Update();
                }
                //вкладка "ОТЧЁТЫ"
                else if (btn.Name == "btn_Reports_All" || btn.Name == "btn_Reports_Army" || btn.Name == "btn_Reports_Trading" ||
                         btn.Name == "btn_Reports_Other" || btn.Name == "btn_Reports_Archive" || btn.Name == "btn_Reports_Neighborhood") {
                    btn_Reports_All.Tag = btn_Reports_Army.Tag = btn_Reports_Trading.Tag = btn_Reports_Other.Tag = btn_Reports_Archive.Tag = btn_Reports_Neighborhood.Tag = 0;
                    btn.Tag = 1;//отмечаем нажатую кнопку
                    btn_Reports_All.BackColor = btn_Reports_Army.BackColor = btn_Reports_Trading.BackColor =
                    btn_Reports_Other.BackColor = btn_Reports_Archive.BackColor = btn_Reports_Neighborhood.BackColor = Color.DarkGray;
                    btn_Reports_All.ForeColor = btn_Reports_Army.ForeColor = btn_Reports_Trading.ForeColor =
                    btn_Reports_Other.ForeColor = btn_Reports_Archive.ForeColor = btn_Reports_Neighborhood.ForeColor = Color.Black;
                    btn.BackColor = Color.Black; btn.ForeColor = Color.Transparent;
                    _Update();
                }
                //вкладка "СООБЩЕНИЯ"
                else if (btn.Name == "btn_Messages_Incoming" || btn.Name == "btn_Messages_Outgoing" ||
                    btn.Name == "btn_Messages_Archive" || btn.Name == "btn_Messages_Write") {
                    btn_Messages_Incoming.Tag = btn_Messages_Outgoing.Tag = btn_Messages_Archive.Tag = btn_Messages_Write.Tag = 0;
                    btn.Tag = 1;//отмечаем нажатую кнопку
                    btn_Messages_Incoming.BackColor = btn_Messages_Outgoing.BackColor = btn_Messages_Archive.BackColor = btn_Messages_Write.BackColor = Color.DarkGray;
                    btn_Messages_Incoming.ForeColor = btn_Messages_Outgoing.ForeColor = btn_Messages_Archive.ForeColor = btn_Messages_Write.ForeColor = Color.Black;
                    btn.BackColor = Color.Black; btn.ForeColor = Color.Transparent;
                        grid_Messages.Columns[3].HeaderText = (int)btn_Messages_Outgoing.Tag == 1 ?
                            $" {LANGUAGES.Messages[20]} "/*Получатель:*/ : $" {LANGUAGES.Messages[14]} "/*Отправитель:*/;
                    _Update();
                    if (btn.Name == "btn_Messages_Write") {
                        Location_Recipient = new Point(-1, -1);/*default*/
                        //string newLine = Environment.NewLine;
                        string endl = "\r\n"; Create_Text_Message_Write();
                        Text_Message_Write.Text = $"{endl}_________________________{endl}{LANGUAGES.Messages[22]/*Пишет:*/} {Player.Nick_Name}";
                        WinDlg_Message(WindowMessage: Window_Message.Write);
                    }
                }
                //кнопка "удалить" сообщение в окне сообщения в режиме "ЧТЕНИЕ"
                else if (btn.Name == "btn_DeleteMessage") {
                    if (MessageIndex > -1) {
                        GAME.Messages.LIST.Remove((TMessage.TData)grid_Messages.Rows[MessageIndex].Cells[0].Tag);
                        grid_Messages.Rows.RemoveAt(MessageIndex);
                        MarkAsRead = false;/*помечаем не прочитанным, чтобы не помечать удалённое сообщение*/
                        var R = TableHead_Message.Rows; string msg = R.Count >= 3 ? 
                            $"{R[0].Cells[0].Value} {R[0].Cells[1].Value}\n" +
                            $"{R[1].Cells[0].Value} {R[1].Cells[1].Value}\n" +
                            $"{R[2].Cells[0].Value} {R[2].Cells[1].Value}" : "";
                        MessageBox.Show($"{LANGUAGES.Messages[16]/*Сообщение удалено*/}:\n\n{msg}");
                    } else MessageBox.Show(LANGUAGES.Messages[17]/*Сообщение не было удалено!*/);
                    Form_Message.Close();
                    MessageIndex = -1;
                }
                //кнопка "отметить непрочитанным" в окне сообщения в режиме "ЧТЕНИЕ"
                else if (btn.Name == "btn_MarkAsNotRead_Message") {
                    if (MessageIndex > -1) {
                        Messages_ReadNotRead(false, MessageIndex);//помечаем выбранное сообщение как не прочитанное
                        ((TMessage.TData)grid_Messages.Rows[MessageIndex].Cells[0].Tag).Read = MarkAsRead = false;//помечаем не прочитанным
                        Form_Message.Close();
                    }
                }
                //кнопка "ответить" в окне сообщения в режиме "ЧТЕНИЕ"
                else if (btn.Name == "btn_Answer_Message") {
                    if (MessageIndex > -1) {
                        string endl = "\r\n"; Create_Text_Message_Write();
                        Text_Message_Write.Text = endl + "_________________________" + endl +
                            LANGUAGES.Messages[22]/*Пишет:*/ + " " + Player.Nick_Name + endl + endl +
                            ((TMessage.TData)grid_Messages[0, MessageIndex].Tag).Text;
                        WinDlg_Message(MessageIndex, (TMessage.TData)grid_Messages[0, MessageIndex].Tag, WindowMessage: Window_Message.Write);
                    }
                }
                //кнопка "написать сообщение" в окне профиля игрока
                else if (btn.Name == "btn_WriteMessage") {
                    string endl = "\r\n"; Create_Text_Message_Write();
                    Text_Message_Write.Text = $"{endl}_________________________{endl}{LANGUAGES.Messages[22]/*Пишет:*/} {Player.Nick_Name}";
                    WinDlg_Message(WindowMessage: Window_Message.Write);
                }
                //кнопка "отправить" сообщение в окне сообщения в режиме "ЗАПИСИ"
                else if (btn.Name == "btn_Send") {
                    if (TableHead_MessageWrite[1, 0].Value.ToString() == "" || TableHead_MessageWrite[1, 1].Value.ToString() == "" ||
                        Text_Message_Write.Text == "") {
                        MessageBox.Show(LANGUAGES.Messages[23]/*Невозможно отправить сообщение!*/ + "\n"
                            + LANGUAGES.Messages[24]/*Эти поля не могут быть пустыми: 'получатель', 'тема', 'текст сообщения'.*/);
                        return;
                    } else { //добавляем сообщение
                        GAME.Messages.Add(Type_Message.Outgoing, Player.VillageList[0].Coordinates_Cell, Location_Recipient,
                            DepthMessage + 1, false, false, $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}",
                            $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}", TopicMessage, Text_Message_Write.Text);
                    }
                    MessageIndex = -1;
                    Control_Click(btn_Messages_Outgoing, new EventArgs());//выбираем фильтр "отправленные"
                    _Update();//обновляем таблицу сообщений если выбран фильтр "отправленные"
                    Form_Message.Close();
                }
                //Окно winDlg_LevelUp_Builds(...); Повышение уровня выбранной постройки
                else if (btn.Name == "btn_LevelUp") {
                    btn_Construction.Enabled = true;
                    Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);

                    //проверка лимита одновременных построек
                    int CountConstruction = GAME.Event_Stack.Constructions_Count(p);
                    int MaxCountConstruction = GAME.Get_MaxCount_Building_InOfConstructionQueue(Player.Folk_Name);
                    if (CountConstruction >= MaxCountConstruction) {
                        //Исчерпан лимит одновременных построек. Добавление события в стек не произошло.
                        MessageBox.Show(LANGUAGES.RESOURSES[100] + "\nCountConstruction = " + CountConstruction + "\n" + "MaxCountConstruction = " + MaxCountConstruction);
                        return;
                    }

                    int NumberSlot = (int)btn.Tag;
                    int lvl = Актуальный_Уровень_С_Учётом_Строительства_И_Сноса(NumberSlot);
                    Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID;
                    bool Строится = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction;
                    Buildings ID_Строящегося = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction;
                    int Бонус_Главного_Здания = Player.VillageList[Player.ActiveIndex].BonusOfTime_Construction;
                    if (ID == Buildings.ПУСТО) { if (Строится) { ID = ID_Строящегося; } else ID = (Buildings)get_Number(); }
                    var Build_Next = GAME.Build[(int)ID].Information[lvl + 1];//информация запускаемой постройки
                    Res RES = Player.VillageList[Player.ActiveIndex].ResourcesInStorages;//ресурсы на складе
                    int max = GAME.Event_Stack.MaxTime(p, Type_Event.Construction); 
                    int time = max + (int)(GAME.Build[(int)ID].Information[lvl + 1].Time_Construction / 100.0 * Бонус_Главного_Здания);
                    //проверка ресурсов на складе
                    if (RES.wood < Build_Next.Construction_Costs.wood || RES.clay < Build_Next.Construction_Costs.clay ||
                        RES.iron < Build_Next.Construction_Costs.iron || RES.crop < Build_Next.Construction_Costs.crop) {
                        MessageBox.Show(LANGUAGES.RESOURSES[135]);//Невозможно построить. Не хватает ресурсов в хранилищах. \n В окне строительства, ресурсы которых не хватает, отмечены красным цветом.
                        //return; //ОТКЛЮЧЕНО ДЛЯ ТЕСТА, ЧТОБЫ ВСЁ РАВНО МОЖНО БЫЛО ЗАПУСТИТЬ ПОСТРОЙКУ, А НА СКЛАДЕ БУДУТ НУЛИ ТАМ ГДЕ БЫЛА НЕХВАТКА РЕСУРСОВ
                    }

                    //добавляем постройку соответствующего уровня в выбранный слот
                    if (GAME.Event_Stack.Add(time, Type_Event.Construction, Type_Movement.None, 
                            Player.Folk_Name, p, p, null, null, NumberSlot, lvl + 1, ID, false)) {
                        Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction = true;//false ставится в таймере
                        Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction = ID;
                        if ((int)btn.Tag < 18) Update_Buttons_Level_Slots(Slots.Resources);
                        else Update_Buttons_Level_Slots(Slots.Village);
                        form_LevelUp.Close();
                    }

                    //вычет ресурсов из хранилищ
                    RES.wood -= Build_Next.Construction_Costs.wood; RES.clay -= Build_Next.Construction_Costs.clay;
                    RES.iron -= Build_Next.Construction_Costs.iron; RES.crop -= Build_Next.Construction_Costs.crop;
                    Player.VillageList[Player.ActiveIndex].ResourcesInStorages = new Res(RES.wood, RES.clay, RES.iron, RES.crop,
                                                                Player.VillageList[Player.ActiveIndex].ResourcesInStorages.gold);

                    //коррекция положения панели очереди строительства
                    if (tabControl.SelectedIndex == 1) Panel_Construction.Location(LC.pc_X, LC.pc_Y);
                    else if (tabControl.SelectedIndex == 2) Panel_Construction.Location = new Point(ToCSR(50), Panel_Construction.Parent.Height - Panel_Construction.Height - ToCSR(80));
                }
                //Окно winDlg_LevelUp_Builds(...); снос выбранной постройки на 1 уровень
                else if (btn.Name == "btn_LevelDown_Destroy") {
                    btn_Construction.Enabled = true;
                    int lvl = Player.VillageList[Player.ActiveIndex].Slot[27].Level;//ГЗ
                    if (lvl >= 1/*10*/) { //можно сносить здания если Главное Здание от 10 уровня и выше
                        Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);

                        //проверка лимита одновременных построек
                        int CountConstruction = GAME.Event_Stack.Constructions_Count(p);
                        int MaxCountConstruction = GAME.Get_MaxCount_Building_InOfConstructionQueue(Player.Folk_Name);
                        if (CountConstruction >= MaxCountConstruction) {
                            //Исчерпан лимит одновременных построек. Добавление события в стек не произошло.
                            MessageBox.Show(LANGUAGES.RESOURSES[100] + "\nCountConstruction = " + CountConstruction + "\n" + "MaxCountConstruction = " + MaxCountConstruction);
                            return;
                        }

                        int NumberSlot = (int)btn.Tag;
                        lvl = Актуальный_Уровень_С_Учётом_Строительства_И_Сноса(NumberSlot);
                        Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID;
                        bool Строится = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction;
                        Buildings ID_Строящегося = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction;
                        int Бонус_Главного_Здания = Player.VillageList[Player.ActiveIndex].BonusOfTime_Construction;
                        if (ID == Buildings.ПУСТО) { if (Строится) { ID = ID_Строящегося; } else ID = (Buildings)get_Number(); } 
                        if (lvl >= 1) {
                            int max = GAME.Event_Stack.MaxTime(p, Type_Event.Construction);
                            int time = max + (int)(GAME.Build[(int)ID].Information[lvl].Time_Construction / 100.0 * Бонус_Главного_Здания);
                            //сносим постройку на 1 уровень в выбранном слоте
                            if (GAME.Event_Stack.Add(time, Type_Event.Construction, Type_Movement.None,
                                    Player.Folk_Name, p, p, null, null, NumberSlot, lvl - 1, ID, true)) {
                                //строится и ID_строящегося актуально и для сноса.
                                //по сути снос - это то же самое возведение постройки только в обратную сторону.
                                Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction = true;//false ставится в таймере
                                Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction = ID;
                                if ((int)btn.Tag < 18) Update_Buttons_Level_Slots(Slots.Resources);
                                else Update_Buttons_Level_Slots(Slots.Village);
                                form_LevelUp.Close();
                            }
                        }
                    }
                    //коррекция положения панели очереди строительства
                    if (tabControl.SelectedIndex == 1) Panel_Construction.Location(LC.pc_X, LC.pc_Y);
                    else if (tabControl.SelectedIndex == 2) Panel_Construction.Location = new Point(ToCSR(50), Panel_Construction.Parent.Height - Panel_Construction.Height - ToCSR(80));
                }
                //ЗАВЕРШИТЬ ВСЕ СТРОЙКИ МГНОВЕННО
                else if (btn.Name == "btn_Construction") {
                    //проверка ресурсов на складе
                    Res RES = Player.VillageList[Player.ActiveIndex].ResourcesInStorages;//ресурсы на складе
                    if (RES.gold < GAME.CostInGoldForInstantCompletionOfConstruction) {
                        MessageBox.Show(LANGUAGES.RESOURSES[136]);//Невозможно завершить строительство. В казне не хватает золота.
                        //return; //ОТКЛЮЧЕНО ДЛЯ ТЕСТА, ЧТОБЫ ВСЁ РАВНО МОЖНО БЫЛО ЗАВЕРШИТЬ СТРОИТЕЛЬСТВО МОМЕНТАЛЬНО, А В СОКРОВИЩНИЦЕ БУДЕТ НОЛЬ ЗОЛОТА ИЗ-ЗА НЕХВАТКИ
                    }
                    //алгоритм мгновенного завершения всех строек
                    btn_Construction.Enabled = false;
                    Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);
                    for (int i = 0; i < GAME.Event_Stack.Stack.Count; i++) {
                        if (GAME.Event_Stack.Stack[i].Cell_Start == p &&
                            GAME.Event_Stack.Stack[i].TypeEvent == Type_Event.Construction
                           ) GAME.Event_Stack.Stack[i].timer = 1;/*завершаем строительство*/
                    }
                    //вычет ресурсов из хранилищ (золота из казны)
                    RES.gold -= GAME.CostInGoldForInstantCompletionOfConstruction;
                    Player.VillageList[Player.ActiveIndex].ResourcesInStorages = new Res(RES.wood, RES.clay, RES.iron, RES.crop, RES.gold);
                }
            } else if (sender is PictureBox pb) {
                //Panel_Construction :: [X] отмена строительства постройки. красный крестик слева
                if (pb.Name == "ico2") {
                    var LinkStack = (TEvent_Stack.TMultiEvent)pb.Tag;//ссылка на строку в стеке List<T>
                    var Event = GAME.Event_Stack;
                    for (int i = 0; i < Event.Stack.Count; i++) {
                        if (LinkStack == Event.Stack[i]) {
                            //сносим последнее запущенное строение
                            Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);
                            int EventSlot = Event.Stack[i].Slot;
                            Buildings EventID = Event.Stack[i].ID;
                            bool Снос = Event.Stack[i].Destruction;
                            var Village = Player.VillageList[Player.ActiveIndex];
                            var Village_Slot = Village.Slot[EventSlot];
                            int lvl = Актуальный_Уровень_С_Учётом_Строительства_И_Сноса(EventSlot);
                            Village_Slot.ProcessOfConstruction = GAME.Event_Stack.IsMoreThanOneConstructionInTheSlot(p, EventSlot);
                            if (lvl <= 1) { Village_Slot.ID_ProcessOfConstruction = Buildings.ПУСТО; }
                            //возврат ресурсов в хранилища
                            if (!Снос) { 
                                var Build_Next = GAME.Build[(int)EventID].Information[lvl];
                                Res add = Village.ResourcesInStorages;
                                if (lvl == 1) { //на 1 уровне возврат 100%
                                    add.wood += Build_Next.Construction_Costs.wood; add.clay += Build_Next.Construction_Costs.clay;
                                    add.iron += Build_Next.Construction_Costs.iron; add.crop += Build_Next.Construction_Costs.crop;
                                } else if (lvl >= 2) { //на 2 и выше возвращается дельта: lvl - (lvl-1)
                                    var Build_now = GAME.Build[(int)EventID].Information[lvl].Construction_Costs;
                                    var Build_Prev = GAME.Build[(int)EventID].Information[lvl - 1].Construction_Costs;
                                    var Delta = new Res(Build_now.wood - Build_Prev.wood, Build_now.clay - Build_Prev.clay,
                                                        Build_now.iron - Build_Prev.iron, Build_now.crop - Build_Prev.crop,
                                                        Build_now.gold - Build_Prev.gold);
                                    add.wood += Delta.wood; add.clay += Delta.clay; add.iron += Delta.iron; add.crop += Delta.crop;
                                }
                                Village.ResourcesInStorages = new Res(add.wood, add.clay, add.iron, add.crop, Village.ResourcesInStorages.gold);
                            }
                            Event.Stack.RemoveAt(i); pb.Tag = null;
                            break;
                        }
                    }
                }
                //кнопки выбора вкладок
                else if (pb.Name == "pb_Button_Resourses") {
                    if (tabControl.SelectedIndex == 1 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке ресурсных полей
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picResource_1.png");//green
                    pb_Button_Resourses.Tag = 1; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
                    pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;
                    tabControl.SelectedIndex = 1;//к ресурсным полям
                } else pb_Button_Resourses.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picResource_0.png");//gray
                if (pb.Name == "pb_Button_Village") {
                    if (tabControl.SelectedIndex == 2 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке деревни
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picVillage_1.png");//green
                    pb_Button_Resourses.Tag = 0; pb_Button_Village.Tag = 1; pb_Button_Map.Tag = 0;
                    pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;
                    tabControl.SelectedIndex = 2;//к деревне
                } else pb_Button_Village.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picVillage_0.png");//gray
                if (pb.Name == "pb_Button_Map") {
                    if (tabControl.SelectedIndex == 3 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке карты
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMap_1.png");//green
                    pb_Button_Resourses.Tag = 0; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 1;
                    pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;
                    tabControl.SelectedIndex = 3;//к карте
                } else pb_Button_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMap_0.png");//gray
                if (pb.Name == "pb_Button_Statistics") {
                    if (tabControl.SelectedIndex == 4 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке статистики
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picStatistics_1.png");//green
                    pb_Button_Resourses.Tag = 0; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
                    pb_Button_Statistics.Tag = 1; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;
                    tabControl.SelectedIndex = 4;//к статистике
                } else pb_Button_Statistics.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picStatistics_0.png");//gray
                if (pb.Name == "pb_Button_Reports") {
                    if (tabControl.SelectedIndex == 5 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке отчёты
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picReports_1.png");//green
                    pb_Button_Resourses.Tag = 0; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
                    pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 1; pb_Button_Messages.Tag = 0;
                    tabControl.SelectedIndex = 5;//к отчётам
                } else pb_Button_Reports.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picReports_0.png");//gray
                if (pb.Name == "pb_Button_Messages") {
                    if (tabControl.SelectedIndex == 6 && (int)pb.Tag == 1) return;//нет смысла кликать мы и так на вкладке сообщения
                    pb.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMessages_1.png");//green
                    pb_Button_Resourses.Tag = 0; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
                    pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 1;
                    tabControl.SelectedIndex = 6;//к сообщениям
                } else pb_Button_Messages.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMessages_0.png");//gray
                tabControl_SelectedIndexChanged(sender, e);
            } else if (sender is Label lb) {
                //обработка клика по нику в окне информации о выбранной ячейке
                if (lb.Name == "lb_Village7") { 
                    //Form_InfoCell.Close();
                    WinDlg_AccountProfile((TPlayer)lb.Tag);//открыть окно профиля аккаунта
                }
            }
            else return;
        }

        public void Control_MouseEnter(object sender, EventArgs e) {
            //break в switch ТОЛЬКО У ГЛАВНОГО МЕНЮ, остальные кейсы завершаются через return
            var Ctrl = sender as Control;
            switch (Ctrl.Name) {
                //вкладка: "главное меню"
                case "btn_Rome":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Romans_Color.jpg");
                    rich_info.LoadFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/MENU_Romans.rtf");
                    statusStrip.Items[0].Text = LANGUAGES.RESOURSES[21];//Римляне
                    break;
                case "btn_Gauls":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Gauls_Color.jpg");
                    rich_info.LoadFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/MENU_Gauls.rtf");
                    statusStrip.Items[0].Text = LANGUAGES.RESOURSES[23];//Галлы
                    break;
                case "btn_Teutons":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Teutons_Color.jpg");
                    rich_info.LoadFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/MENU_Teutons.rtf");
                    statusStrip.Items[0].Text = LANGUAGES.RESOURSES[22];//Германцы
                    break;
                //текстовая метка с игровым ником в окне информации о выбранной ячейке
                case "lb_Village7": Ctrl.ForeColor = Color.Red; return;
                //кнопки изменения скорости игры
                case "btn_SpeedGame1": case "btn_SpeedGame2": case "btn_SpeedGame3": case "btn_SpeedGame4":
                case "btn_SpeedGame5": case "btn_SpeedGame6":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/" + Ctrl.Name + "_" + ((int)Ctrl.Tag + 2) + ".png");
                    Text = Player.Hero.Name;
                    return;
                //кнопка героя на портрете
                case "btn_Hero": //btn.Contour_Angles(4, 10, Color.Red);
                    //алгоритм рисует на кнопке эффект изменённого заднего фона героя в круге
                    /*кнопка*/Bitmap bmp = new Bitmap(Ctrl.Width, Ctrl.Height); Ctrl.DrawToBitmap(bmp, Ctrl.ClientRectangle);
                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int length = bmp.Width * bmp.Height * 4; byte[] rgba = new byte[length];
                    Marshal.Copy(data.Scan0, rgba, 0, length);
                    /*Parent*/Bitmap BMP = new Bitmap(Ctrl.Parent.Width, Ctrl.Parent.Height); Ctrl.Parent.DrawToBitmap(BMP, Ctrl.Parent.ClientRectangle);
                    BitmapData DATA = BMP.LockBits(new Rectangle(0, 0, BMP.Width, BMP.Height), ImageLockMode.ReadWrite, BMP.PixelFormat);
                    int LENGTH = BMP.Width * BMP.Height * 4; byte[] RGBA = new byte[LENGTH];
                    Marshal.Copy(DATA.Scan0, RGBA, 0, LENGTH);
                    for (int y = 0; y < Ctrl.Height; y++) for (int x = 0; x < Ctrl.Width; x++) {
                        int X = x + Ctrl.Left, Y = y + Ctrl.Top; int i = (y * bmp.Width + x) * 4; int I = (Y * BMP.Width + X) * 4;
                        //эталон цвета(206, 185, 154)
                        if ((RGBA[I + 2] >= 204 && RGBA[I + 2] <= 208)/*R*/ &&
                            (RGBA[I + 1] >= 183 && RGBA[I + 1] <= 187)/*G*/ &&
                            (RGBA[I + 0] >= 152 && RGBA[I + 0] <= 156)/*B*/) {
                                rgba[i + 3] = 255/*alpha*/;  rgba[i + 2] = 80;/*R*/
                                rgba[i + 1] = 115;/*G*/      rgba[i + 0] = 50;/*B*/
                        }
                    } Marshal.Copy(rgba, 0, data.Scan0, length); bmp.UnlockBits(data); BMP.UnlockBits(DATA);
                    Ctrl.BackgroundImage = bmp;
                    break;
                //кнопки переключения между вкладками
                case "pb_Button_Resourses": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                case "pb_Button_Village": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                case "pb_Button_Map": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                case "pb_Button_Statistics": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                case "pb_Button_Reports": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                case "pb_Button_Messages": Ctrl.Contour_Angles(2, 20, Color.FromArgb(255, 0, 0)); return;
                //кнопки смещения карты по осям
                case "btn_Map_Left_X": pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX_Left_X.png"); return;
                case "btn_Map_Right_X": pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX_Right_X.png"); return;
                case "btn_Map_Up_Y": pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX_Up_Y.png"); return;
                case "btn_Map_Down_Y": pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX_Down_Y.png"); return;
                //кнопки вкладки "Статистика"
                case "btn_Statistics_Players": case "btn_Statistics_Alliances": case "btn_Statistics_Villages":
                case "btn_Statistics_Heroes": case "btn_Statistics_Wonders":
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    else { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    return;
                //кнопки вкладки "Отчёты"
                case "btn_Reports_All": case "btn_Reports_Army": case "btn_Reports_Trading":
                case "btn_Reports_Other": case "btn_Reports_Archive": case "btn_Reports_Neighborhood":
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    else { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    return;
                //кнопки вкладки "Сообщения"
                case "btn_Messages_Incoming": case "btn_Messages_Outgoing":
                case "btn_Messages_Archive": case "btn_Messages_Write":
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    else { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    return;
                //кнопка "удалить" сообщение в окне чтения сообщений
                case "btn_DeleteMessage": int th = Ctrl.Height / 15 > 0 ? Ctrl.Height / 15 : 1;
                    Ctrl.Contour_Angles((byte)(th + 1), 25, Color.FromArgb(255, 0, 0));
                    return;
                //кнопка "написать сообщение" в окне профиля игрока
                case "btn_WriteMessage": Ctrl.ForeColor = Color.FromArgb(240, 160, 80); return;
                //массивы кнопок над слотами запуска уровня ресурсных и деревенских построек
                case "Button_Slot_resources": case "Button_Slot_builds":
                    //КОД МОЕЙ ВСПЛЫВАЮЩЕЙ ПОДСКАЗКИ:
                    Control parent;
                    if (Ctrl.Name == "Button_Slot_resources") {
                        ((Button)Ctrl).Button_Styles(Color.Transparent, ((Button)Ctrl).FlatAppearance.MouseDownBackColor, Color.Transparent,
                                       ((Button)Ctrl).FlatAppearance.BorderColor, Ctrl.ForeColor, 2); parent = bg_Resources;
                    } else {
                        Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/id_build_move.png"); parent = bg_Village;
                        if (Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ID == Buildings.ПУСТО)
                            if (Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ProcessOfConstruction == false)
                                img_Slot_builds[(int)Ctrl.Tag - 18].BackgroundImage = Image.FromFile("DATA_BASE/IMG/building/slot_build_move.png");
                    }

                    //Пользовательская отрисовка ресурсных всплывающих подсказок
                    int lvl = Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].Level;
                    Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ID;
                    string Имя = LANGUAGES.buildings[(int)Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ID];
                    Color ValueTextColor = Color.White;
                    statusStrip.Items[2].Text = "ID: " + ((int)Ctrl.Tag).ToString();
                    if (pb_tool_tip_BackGround == null) {
                        //панель
                        pb_tool_tip_BackGround = new PictureBox { Parent = parent, BackgroundImageLayout = ImageLayout.Stretch, };
                        //первые 2 строки с инфой
                        lb_tt_info = new Label[2];
                        for (int i = 0; i < lb_tt_info.Length; i++) {
                            FontStyle fs; if (i == 0) fs = FontStyle.Bold; else fs = FontStyle.Regular;
                            lb_tt_info[i] = new Label {
                                Parent = pb_tool_tip_BackGround, AutoSize = true,
                                ForeColor = Color.FromArgb(255, 255, 255), BackColor = Color.Transparent,
                                Font = new Font(Font.FontFamily, ToCSR(10), fs), 
                            }; lb_tt_info[i].BringToFront();
                        }
                        //массив ресурсных картинок-пиктограмм
                        //[0] = wood    [1] = clay      [2] = iron      [3] = crop
                        pb_tt_pr = new PictureBox[4];
                        for (int i = 0; i < pb_tt_pr.Length; i++) {
                            pb_tt_pr[i] = new PictureBox {
                                Parent = pb_tool_tip_BackGround, BackgroundImageLayout = ImageLayout.Stretch,
                            };
                            pb_tt_pr[i].Size(32, 22);
                            pb_tt_pr[i].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/0" + i + ".png");
                            pb_tt_pr[i].BringToFront();
                        }
                        //массив ресурсных величин для пиктограмм
                        lb_tt_pr = new Label[pb_tt_pr.Length];
                        for (int i = 0; i < lb_tt_pr.Length; i++) {
                            lb_tt_pr[i] = new Label {
                                Parent = pb_tool_tip_BackGround, AutoSize = true,
                                ForeColor = ValueTextColor, BackColor = Color.Transparent, 
                                Font = new Font(Font.FontFamily, ToCSR(10), FontStyle.Regular),
                            }; lb_tt_pr[i].BringToFront();
                        }
                    } pb_tool_tip_BackGround.Visible = true;

                    int MAX_X, MAX_Y;//размер панели всплывающей подсказки
                    lb_tt_info[0].Text = Имя.ToUpper() + ". " + LANGUAGES.RESOURSES[3] + " " + lvl;//Level
                    lb_tt_info[0].Location = new Point(10, 5);
                    lb_tt_info[1].Location = new Point(10, lb_tt_info[0].Top + lb_tt_info[0].Height);
                    uint Max_lvl; bool isBool;//bool = true если подсказка в усечённом варианте (2 строки)
                    if (ID == 0) Max_lvl = 100; else if (ID >= Buildings.Лесопильный_завод && ID <= Buildings.Пекарня) Max_lvl = 5;
                    else if (ID == Buildings.Тайник) Max_lvl = 10;/*тайник 10 ур.*/
                    else if (Player.ActiveIndex == Player.NumberOfCapital) Max_lvl = 20; else Max_lvl = Player.Limit_Lvl;
                    if (ID == Buildings.ПУСТО) { lb_tt_info[1].Text = LANGUAGES.RESOURSES[4]; isBool = true; }//этот слот пустой
                    else if (lvl + 1 <= Max_lvl) {
                        //расходы на строительство до следующего уровня
                        isBool = false;//подробная подсказка
                        lb_tt_info[1].Text = LANGUAGES.RESOURSES[5] + " " + (lvl + 1) + ":";//Расходы на строительство до уровня
                        var PRIS = Player.VillageList[Player.ActiveIndex].ResourcesInStorages;
                        var CC = GAME.Build[(int)ID].Information[lvl + 1].Construction_Costs;
                        if (PRIS.wood < CC.wood) lb_tt_pr[0].ForeColor = Color.Red; else lb_tt_pr[0].ForeColor = ValueTextColor;
                        if (PRIS.clay < CC.clay) lb_tt_pr[1].ForeColor = Color.Red; else lb_tt_pr[1].ForeColor = ValueTextColor;
                        if (PRIS.iron < CC.iron) lb_tt_pr[2].ForeColor = Color.Red; else lb_tt_pr[2].ForeColor = ValueTextColor;
                        if (PRIS.crop < CC.crop) lb_tt_pr[3].ForeColor = Color.Red; else lb_tt_pr[3].ForeColor = ValueTextColor;
                        //wood
                        /*pic loc*/
                        pb_tt_pr[0].Location = new Point(10, lb_tt_info[1].Top + lb_tt_info[1].Height + 5);
                        /*txt*/lb_tt_pr[0].Text = toTABString(CC.wood.ToString());
                        /*txt loc*/lb_tt_pr[0].Location = new Point(pb_tt_pr[0].Left + pb_tt_pr[0].Width + 5, pb_tt_pr[0].Top);
                        //clay
                        /*pic loc*/pb_tt_pr[1].Location = new Point(lb_tt_pr[0].Left + lb_tt_pr[0].Width + ToCSR(20), pb_tt_pr[0].Top);
                        /*txt*/lb_tt_pr[1].Text = toTABString(CC.clay.ToString());
                        /*txt loc*/lb_tt_pr[1].Location = new Point(pb_tt_pr[1].Left + pb_tt_pr[1].Width + 5, pb_tt_pr[0].Top);
                        //iron
                        /*pic loc*/pb_tt_pr[2].Location = new Point(lb_tt_pr[1].Left + lb_tt_pr[1].Width + ToCSR(20), pb_tt_pr[0].Top);
                        /*txt*/lb_tt_pr[2].Text = toTABString(CC.iron.ToString());
                        /*txt loc*/lb_tt_pr[2].Location = new Point(pb_tt_pr[2].Left + pb_tt_pr[2].Width + 5, pb_tt_pr[0].Top);
                        //crop
                        /*pic loc*/pb_tt_pr[3].Location = new Point(lb_tt_pr[2].Left + lb_tt_pr[2].Width + ToCSR(20), pb_tt_pr[0].Top);
                        /*txt*/lb_tt_pr[3].Text = toTABString(CC.crop.ToString());
                        /*txt loc*/lb_tt_pr[3].Location = new Point(pb_tt_pr[3].Left + pb_tt_pr[3].Width + 5, pb_tt_pr[0].Top);
                    } else { lb_tt_info[1].Text = LANGUAGES.RESOURSES[6];/*Это здание отстроено полностью.*/ isBool = true; }
                    if (isBool) { //усечённая подсказка в 2 строки
                        for (int i = 0; i < pb_tt_pr.Length; i++) { pb_tt_pr[i].Visible = false; lb_tt_pr[i].Visible = false; }
                        //вычисляем ширину панели
                        MAX_X = lb_tt_info[0].Left + lb_tt_info[0].Width;
                        if (lb_tt_info[1].Left + lb_tt_info[1].Width > MAX_X) MAX_X = lb_tt_info[1].Left + lb_tt_info[1].Width;
                        //вычисляем высоту панели
                        MAX_Y = lb_tt_info[1].Top + lb_tt_info[1].Height;
                    } else { //подробная подсказка
                        for (int i = 0; i < pb_tt_pr.Length; i++) { pb_tt_pr[i].Visible = true; lb_tt_pr[i].Visible = true; }
                        //вычисляем ширину панели
                        MAX_X = lb_tt_info[0].Left + lb_tt_info[0].Width;
                        if (lb_tt_info[1].Left + lb_tt_info[1].Width > MAX_X) MAX_X = lb_tt_info[1].Left + lb_tt_info[1].Width;
                        if (lb_tt_pr[3].Left + lb_tt_pr[3].Width > MAX_X) MAX_X = lb_tt_pr[3].Left + lb_tt_pr[3].Width;
                        //вычисляем высоту панели
                        MAX_Y = pb_tt_pr[0].Top + pb_tt_pr[0].Height;
                    }

                    Point loc = new Point(0, 0);
                    if (Ctrl.Name == "Button_Slot_resources") loc = new Point(Ctrl.Left, Ctrl.Top + Ctrl.Height + ToCSR(5));
                    else if (Ctrl.Name == "Button_Slot_builds") if (Ctrl.Parent == bg_Village)
                            loc = new Point(Ctrl.Left, Ctrl.Top + Ctrl.Height + ToCSR(5));
                            else loc = new Point(Ctrl.Parent.Parent.Left + Ctrl.Parent.Left + Ctrl.Left,
                                Ctrl.Parent.Parent.Top + Ctrl.Parent.Top + Ctrl.Top + Ctrl.Height + ToCSR(5));
                    pb_tool_tip_BackGround.Location = loc;
                    pb_tool_tip_BackGround.Size = new Size(MAX_X + ToCSR(20), MAX_Y + ToCSR(10));
                    pb_tool_tip_BackGround.BackgroundImage = null;

                    pb_tool_tip_BackGround.Transparent(Color.FromArgb(0, 0, 0), 180);//полупрозрачка
                    pb_tool_tip_BackGround.Contour_Solid(3, Color.FromArgb(0, 0, 0));//рамка
                    pb_tool_tip_BackGround.BringToFront();
                return;
                default: return;
            }
            //вкладка: "главное меню"
            Global.timer1_param = 0; Ctrl.ForeColor = Color.Red; rich_info.Height = 1; rich_info.Visible = true;
            tm_Animate_MainMenu.Interval = 1; tm_Animate_MainMenu.Enabled = true; tm_Animate_MainMenu.Start();
        }

        public void Control_MouseLeave(object sender, EventArgs e) {
            //break в switch ТОЛЬКО У ГЛАВНОГО МЕНЮ, остальные кейсы завершаются через return
            var Ctrl = sender as Control;
            switch (Ctrl.Name) {
                //вкладка: "главное меню"
                case "btn_Rome": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Romans_Gray.jpg"); break;
                case "btn_Gauls": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Gauls_Gray.jpg"); break;
                case "btn_Teutons": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/MENU_Teutons_Gray.jpg"); break;
                //текстовая метка с игровым ником в окне информации о выбранной ячейке
                case "lb_Village7": Ctrl.ForeColor = Color.FromArgb(60, 130, 40);/*green*/ return;
                //кнопки изменения скорости игры
                case "btn_SpeedGame1": case "btn_SpeedGame2": case "btn_SpeedGame3": case "btn_SpeedGame4":
                case "btn_SpeedGame5": case "btn_SpeedGame6":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/" + Ctrl.Name + "_" + (int)Ctrl.Tag + ".png");
                    return;
                //кнопка героя на портрете
                case "btn_Hero": Ctrl.BackgroundImage = null; break;
                //кнопки переключения между вкладками
                case "pb_Button_Resourses": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picResource_" + (int)Ctrl.Tag + ".png"); return;
                case "pb_Button_Village": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picVillage_" + (int)Ctrl.Tag + ".png"); return;
                case "pb_Button_Map": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMap_" + (int)Ctrl.Tag + ".png"); return;
                case "pb_Button_Statistics": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picStatistics_" + (int)Ctrl.Tag + ".png"); return;
                case "pb_Button_Reports": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picReports_" + (int)Ctrl.Tag + ".png"); return;
                case "pb_Button_Messages": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMessages_" + (int)Ctrl.Tag + ".png"); return;
                //кнопки смещения карты по осям
                case "btn_Map_Left_X": case "btn_Map_Right_X": case "btn_Map_Up_Y": case "btn_Map_Down_Y":
                    pb_XXXX_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/map/XXXX.png");
                return;
                //ресурсные кнопки
                case "Button_Slot_resources":
                    ((Button)Ctrl).Button_Styles(Color.Transparent, ((Button)Ctrl).FlatAppearance.MouseDownBackColor, Color.Transparent,
                                      ((Button)Ctrl).FlatAppearance.BorderColor, Ctrl.ForeColor, 0);
                    statusStrip.Items[2].Text = ""; pb_tool_tip_BackGround.Dispose(); pb_tool_tip_BackGround = null;
                return;
                //деревенские кнопки
                case "Button_Slot_builds":
                    Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/id_build.png");
                    if (Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ID == Buildings.ПУСТО)
                        if (Player.VillageList[Player.ActiveIndex].Slot[(int)Ctrl.Tag].ProcessOfConstruction == false)
                            img_Slot_builds[(int)Ctrl.Tag - 18].BackgroundImage = Image.FromFile("DATA_BASE/IMG/building/slot_build.png");
                    statusStrip.Items[2].Text = ""; pb_tool_tip_BackGround.Dispose(); pb_tool_tip_BackGround = null;
                return;
                //кнопки вкладки "Статистика"
                case "btn_Statistics_Players": case "btn_Statistics_Alliances": case "btn_Statistics_Villages":
                case "btn_Statistics_Heroes": case "btn_Statistics_Wonders":
                    Ctrl.BackColor = Color.Transparent; Ctrl.ForeColor = Color.Black;
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    else { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    return;
                //кнопки вкладки "Отчёты"
                case "btn_Reports_All": case "btn_Reports_Army": case "btn_Reports_Trading":
                case "btn_Reports_Other": case "btn_Reports_Archive": case "btn_Reports_Neighborhood":
                    Ctrl.BackColor = Color.Transparent; Ctrl.ForeColor = Color.Black;
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    else { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    return;
                //кнопки вкладки "Сообщения"
                case "btn_Messages_Incoming": case "btn_Messages_Outgoing":
                case "btn_Messages_Archive": case "btn_Messages_Write":
                    Ctrl.BackColor = Color.Transparent; Ctrl.ForeColor = Color.Black;
                    if ((int)Ctrl.Tag == 0) { Ctrl.BackColor = Color.DarkGray; Ctrl.ForeColor = Color.Black; }
                    else { Ctrl.BackColor = Color.Black; Ctrl.ForeColor = Color.Transparent; }
                    return;
                //кнопка "удалить" сообщение в окне чтения сообщений
                case "btn_DeleteMessage": Ctrl.BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/ico/delete_message.png"); return;
                //кнопка "написать сообщение" в окне профиля игрока
                case "btn_WriteMessage": Ctrl.ForeColor = Color.FromArgb(100, 150, 0); return;
                default: return;
            }
            statusStrip.Items[0].Text = ""; Ctrl.ForeColor = Color.White;
            Global.timer1_param = 1; tm_Animate_MainMenu.Interval = 1; tm_Animate_MainMenu.Enabled = true; tm_Animate_MainMenu.Start();
        }

        public void Control_MouseMove(object sender, MouseEventArgs e) {
            string SenderName = (sender as Control).Name;
            switch (SenderName) {
                //вкладка: "ресурсные поля". задний фон.
                case "bg_Resources": {
                    //подсветка ресурсных полей при наведении курсором на область выделения
                    int X = e.X; int Y = e.Y;//коры мыши относительно фона вкладки
                    int L, T, W, H; Color min, max;
                    /* ЦЕНТР ДЕРЕВНИ */
                    L = 732; T = 365; W = 125; H = 70; min = Color.FromArgb(65, 80, 35); max = Color.FromArgb(165, 165, 85);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 0 дерево - если 6-ка */
                    L = 535; T = 250; W = 240; H = 65; min = Color.FromArgb(75, 90, 30); max = Color.FromArgb(105, 110, 55);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.45, 0, 0); return; }
                    /* ID 1 зерно - если 6-ка */
                    L = 773; T = 252; W = 100; H = 70; min = Color.FromArgb(225, 200, 70); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.6, 0, 0); return; }
                    /* ID 2 дерево - если 6-ка */
                    L = 850; T = 265; W = 180; H = 75; min = Color.FromArgb(70, 85, 30); max = Color.FromArgb(105, 110, 55);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.45, 0, 0); return; }
                    /* ID 3 железо - если 6-ка */
                    L = 540; T = 290; W = 215; H = 65; min = Color.FromArgb(95, 95, 95); max = Color.FromArgb(145, 145, 145);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 4 глина - если 6-ка */
                    L = 757; T = 317; W = 80; H = 53; min = Color.FromArgb(50, 100, 75); max = Color.FromArgb(200, 145, 95);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 5 глина - если 6-ка */
                    L = 837; T = 317; W = 85; H = 63; min = Color.FromArgb(50, 100, 75); max = Color.FromArgb(200, 145, 90);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 6 зерно - если 6-ка */
                    L = 525; T = 350; W = 110; H = 65; min = Color.FromArgb(235, 165, 60); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.6, 0, 0); return; }
                    /* ID 7 зерно - если 6-ка */
                    L = 635; T = 355; W = 75; H = 60; min = Color.FromArgb(235, 165, 60); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.6, 0, 0); return; }
                    /* ID 8 железо - если 6-ка */
                    L = 895; T = 335; W = 65; H = 70; min = Color.FromArgb(95, 95, 95); max = Color.FromArgb(145, 145, 145);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 9 железо - если 6-ка */
                    L = 960; T = 320; W = 55; H = 85; min = Color.FromArgb(95, 95, 60); max = Color.FromArgb(145, 145, 145);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 10 железо - если 6-ка */
                    L = 1015; T = 330; W = 80; H = 85; min = Color.FromArgb(85, 85, 65); max = Color.FromArgb(145, 145, 145);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 11 зерно - если 6-ка */
                    L = 525; T = 415; W = 110; H = 85; min = Color.FromArgb(235, 190, 85); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.6, 0, 0); return; }
                    /* ID 12 зерно - если 6-ка */
                    L = 635; T = 415; W = 90; H = 65; min = Color.FromArgb(235, 165, 60); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.6, 0, 0); return; }
                    /* ID 13 зерно - если 6-ка */
                    L = 895; T = 410; W = 160; H = 80; min = Color.FromArgb(240, 205, 90); max = Color.FromArgb(255, 230, 110);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 14 дерево - если 6-ка */
                    L = 710; T = 425; W = 175; H = 80; min = Color.FromArgb(0, 0, 0); max = Color.FromArgb(75, 100, 15);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 15 глина - если 6-ка */
                    L = 605; T = 480; W = 180; H = 105; min = Color.FromArgb(65, 55, 55); max = Color.FromArgb(185, 141, 90);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.25, 0, 0); return; }
                    /* ID 16 дерево - если 6-ка */
                    L = 760; T = 505; W = 125; H = 95; min = Color.FromArgb(0, 0, 0); max = Color.FromArgb(75, 100, 15);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.5, 0, 0); return; }
                    /* ID 17 глина - если 6-ка*/
                    L = 865; T = 455; W = 140; H = 120; min = Color.FromArgb(65, 55, 55); max = Color.FromArgb(185, 141, 100);
                    if (X >= L && X < L + W && Y >= T && Y < T + H) { Illumination(L, T, W, H, min, max, 0.25, 0, 0); return; }
                    if (Selected_Cell != null) Selected_Cell.Dispose();
                } break;
            }
        }

        //====================================================== Menu Strip ======================================================
        /// <summary> Метод загружает выбранный аккаунт из меню ИГРА -> Продолжить игру -> игровой аккаунт. </summary>
        public void ContinueGameTSMI_Click(object sender, EventArgs e) {
            //проверка наличия сохранения. если сохранения нет, вывод сообщения, если сохранение есть, осуществится загрузка
            string[] pth = Directory.GetDirectories(GAME.PathFolderSave);
            if (pth.Length == 0) { MessageBox.Show("Error 13.\n" +
                LANGUAGES.RESOURSES[38] + "\n" +//Невозможно продолжить. Отсутствует сохранение игры!
                LANGUAGES.RESOURSES[39] + "\n" +//Сперва создайте аккаунт, выбрав одну из наций. Затем сохраните его.
                LANGUAGES.RESOURSES[40]//Имя сохранения будет совпадать с названием аккаунта.
                ); return; }

            //ставим игру на паузу, останавливаем потоки
            Event_Timer_Frontend.Stop(); Event_Thread_Timer_Backend.Change(0, Timeout.Infinite);

            Cursor.Current = Cursors.WaitCursor;
            pb_Button_Resourses.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picResource_1.png");//green
            pb_Button_Village.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picVillage_0.png");//gray
            pb_Button_Map.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMap_0.png");//gray
            pb_Button_Statistics.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picStatistics_0.png");//gray
            pb_Button_Reports.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picReports_0.png");//gray
            pb_Button_Messages.BackgroundImage = Image.FromFile("DATA_BASE/IMG/button/picMessages_0.png");//gray
            pb_Button_Resourses.Tag = 1; pb_Button_Village.Tag = 0; pb_Button_Map.Tag = 0;
            pb_Button_Statistics.Tag = 0; pb_Button_Reports.Tag = 0; pb_Button_Messages.Tag = 0;

            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;

            using (FileStream fs = new FileStream($"{GAME.PathFolderSave}/[ map ]/map.DAT", FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                    GAME.Map = new TMap(br.ReadInt32(), br.ReadInt32());
            }}

            int Stack_Count;//копируем количество строк в стеке событий из файла
            using (FileStream fs = new FileStream($"{GAME.PathFolderSave}/Event_Stack.DAT", FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) Stack_Count = br.ReadInt32();
            int Report_Count;//копируем количество строк в листе отчёта из файла
            using (FileStream fs = new FileStream($"{GAME.PathFolderSave}/Reports.DAT", FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) Report_Count = br.ReadInt32();
            int Message_Count;//копируем количество строк в листе сообщений из файла
            using (FileStream fs = new FileStream($"{GAME.PathFolderSave}/Messages.DAT", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) Message_Count = br.ReadInt32();
            int Alliance_Count;//копируем количество строк в листе альянсов из файла
            using (FileStream fs = new FileStream($"{GAME.PathFolderSave}/Alliance.DAT", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) Alliance_Count = br.ReadInt32();


            string[] path = Directory.GetDirectories($"{GAME.PathFolderSave}/");
            // 1/8. загрузка игрока
            //========================================================================
            LoadProcess.Restart("1/8. " + LANGUAGES.RESOURSES[81]/*Загрузка игрового аккаунта и его деревень...*/, path.Length);
                if (!Player.LoadAccount(TSMI.Text, $"{GAME.PathFolderSave}/{TSMI.Text}", GAME)) { MessageBox.Show(LANGUAGES.Errors[3]/*Error 2.*/ + " Form1.Player.LoadAccount(...);\n" + LANGUAGES.Errors[4]/*Отсутствуют файлы деревень*/ + " '" + Player.FolderAccount + "'\n" + LANGUAGES.Errors[0]/*Выход из приложения.*/); Environment.Exit(1); }
                Player.Hero.LoadHero($"{GAME.PathFolderSave}/{Player.FolderAccount}");//загрузка героя
                BotList.Clear(); BotList.Add(Player);
            LoadProcess._Update();
            //========================================================================

            // 2/8. загрузка ботов в BotList
            //========================================================================
            string str = "2/8. " + LANGUAGES.RESOURSES[82];/*Загрузка всех аккаунтов и их деревень для ботов...*/
            for (int i = 0; i < path.Length; i++) { if (path[i].Contains("[_")) { TPlayer bot = new TPlayer();
                string account = ""; for (int j = path[i].Length - 1; j > 0; j--) if (path[i][j] == '/') {
                    for (int k = j + 1; k < path[i].Length; k++) account += path[i][k]; break;
                } 
                if (!bot.LoadAccount(account, $"{GAME.PathFolderSave}/{account}", GAME)) { MessageBox.Show(LANGUAGES.Errors[3]/*Error 2.*/ + " Form1.Player.LoadAccount(...);\n" + LANGUAGES.Errors[4]/*Отсутствуют файлы деревень*/ + " '" + bot.FolderAccount + "'\n" + LANGUAGES.Errors[0]/*Выход из приложения.*/); Environment.Exit(1); }
                bot.Hero.LoadHero($"{GAME.PathFolderSave}/{bot.FolderAccount}");//загрузка героя
                BotList.Add(bot);
                LoadProcess.LoadText.Text = str + $" [{i + 1}/{path.Length}]";
                LoadProcess._Update();
            }}
            //========================================================================

            //прописано здесь, потому что в лист Reinforcements строки добавляются в методе LoadVilalge();
            int Reinforcements_Count = 0; for (int i = 0; i < BotList.Count; i++)
                for (int j = 0; j < BotList[i].VillageList.Count; j++)
                    Reinforcements_Count += BotList[i].VillageList[j].Reinforcements.Count;

            // 3/8. загрузка стека событий
            //========================================================================
            LoadProcess.Start("3/8. " + LANGUAGES.RESOURSES[111]/*Загрузка стека событий...*/, Stack_Count);
                GAME.Event_Stack.LoadStack(LoadProcess, GAME.PathFolderSave);
            //=========================================================

            // 4/8. загрузка отчётов
            //========================================================================
            LoadProcess.Start("4/8. " + LANGUAGES.RESOURSES[137]/*Загрузка отчётов игрока...*/, Report_Count);
            GAME.Reports.LoadReports(LoadProcess, GAME.PathFolderSave);
            //=========================================================

            // 5/8. загрузка сообщений
            //========================================================================
            LoadProcess.Start("5/8. " + LANGUAGES.RESOURSES[139]/*Загрузка сообщений игрока...*/, Message_Count);
            GAME.Messages.LoadMessages(LoadProcess, GAME.PathFolderSave);
            //=========================================================

            // 6/8. загрузка альянсов
            //========================================================================
            LoadProcess.Start("6/8. " + LANGUAGES.RESOURSES[149]/*Загрузка альянсов...*/, Alliance_Count);
            GAME.Alliances.LoadAlliances(LoadProcess, GAME.PathFolderSave);
            //========================================================================

            // 7/8. загрузка карты
            //========================================================================
            LoadProcess.Start("7/8. " + LANGUAGES.RESOURSES[83]/*Загрузка карты...*/, GAME.Map.Length_X() * GAME.Map.Length_Y());
                int result = GAME.Map.LoadMap(LoadProcess, GAME.PathFolderSave);
                if (result == -1) { MessageBox.Show(LANGUAGES.Errors[5]/*Error 3.*/ + " Form1.GAME.Map.LoadMap();\n" + LANGUAGES.Errors[6]/*Объект 'Map' не создан!*/ + " " + LANGUAGES.Errors[0]/*Выход из программы.*/); Environment.Exit(1); }
                else if (result == -2) { MessageBox.Show(LANGUAGES.Errors[12]/*Error 14.*/ + " Form1.GAME.Map.LoadMap();\n" + LANGUAGES.Errors[13]/*Отсутствует Файл 'Cell.DAT'.*/ + " " + LANGUAGES.Errors[0]/*Выход из программы.*/); Environment.Exit(1); }

            int ListAlly_Count = 0;
            for (int i = 0; i < GAME.Alliances.LIST.Count; i++) for (int j = 0; j < GAME.Alliances.LIST[i].ListAlly.Count; j++)
                    ListAlly_Count += GAME.Alliances.LIST[i].ListAlly.Count;

            // 8/8. зацепка линков списка для пункта сбора каждому аккаунту каждой деревне
            //========================================================================
            int DataCount = Reinforcements_Count + ListAlly_Count;
            str = "8/8. " + LANGUAGES.RESOURSES[142];/*Загрузка данных...*/
            LoadProcess.Start(str, DataCount);
            int count = 0;
            for (int i = 0; i < BotList.Count; i++) { 
                for (int j = 0; j < BotList[i].VillageList.Count; j++) {
                    //зацепка линков списка для пункта сбора каждому аккаунту каждой деревне
                    for (int k = 0; k < BotList[i].VillageList[j].Reinforcements.Count; k++) {
                        var _ = BotList[i].VillageList[j].Reinforcements[k].CoordCell;
                        BotList[i].VillageList[j].Reinforcements[k].LinkAccount = GAME.Map.Cell[_.X, _.Y].LinkAccount;
                        BotList[i].VillageList[j].Reinforcements[k].LinkVillage = GAME.Map.Cell[_.X, _.Y].LinkVillage;
                        count++;
                    }
                }
                //зацепка линков на альянсы у игроков, если они в них состоят
                for (int l = 0; l < GAME.Alliances.LIST.Count; l++) {
                    for (int m = 0; m < GAME.Alliances.LIST[l].ListAlly.Count; m++) {
                        if (GAME.Alliances.LIST[l].ListAlly[m].CoordinatesCell == BotList[i].VillageList[0].Coordinates_Cell)
                            BotList[i].LinkOnAlliance = GAME.Alliances.LIST[l]; else BotList[i].LinkOnAlliance = null;
                        count++;
                    }
                }
                LoadProcess.LoadText.Text = str + $" [{count + 1}/{DataCount}]";
                LoadProcess._Update();
            }
            LoadProcess.Stop(TLoadProcess.Report.Hide);
            //=========================================================

            //включаем-выключаем всплывающие подсказки. информация содержится в файле аккаунта Interface.txt
            Enable_Visible_Tooltips_TSMI.Checked = GAME.ToolTipFlag;
            for (int i = 0; i < tool_tip.Length; i++) { if (tool_tip[i] != null) tool_tip[i].Active = GAME.ToolTipFlag; }

            statusStrip.Items[0].Text = LANGUAGES.RESOURSES[20] + " " + LANGUAGES.RESOURSES[21 + (int)Player.Folk_Name];//Народ: Имя народа
            statusStrip.Items[1].Text = LANGUAGES.RESOURSES[24] + " " + Player.Nick_Name;//Игровой ник: Имя аккаунта
            statusStrip.Items[0].Width += ToCSR(40); statusStrip.Items[1].Width += ToCSR(40);

            //ДЛЯ ТЕСТА. УДАЛИТЬ ПОТОМ ЭТИ СТРОКИ
            Player.VillageList[Player.ActiveIndex].HourlyProductionResources = new Res(1800, 900, 3600, 450, 200);
            Player.VillageList[Player.ActiveIndex].ResourcesInStorages = new Res(80, 70, 60, 0, 50);

            GAME.Map.Location = Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian;

            if (tabControl.SelectedIndex != 1) tabControl.SelectedIndex = 1;
            else TabControl_SelectedIndexChanged();//к ресурсным полям - последняя строка!

            BackGroundResources();
            SetLanguage();//перевести весь текст

            Cursor.Current = Cursors.Default;

            //снимаем игру с паузы, включаем потоки
            Event_Timer_Frontend.Start(); Event_Thread_Timer_Backend.Change(0, GAME.SpeedGame);
        }

        /// <summary> Метод включает/выключает чеки в меню и обрабатывает логику последствий. </summary>
        private void ToolStripMenuItem_Click(object sender, EventArgs e) {
            var obj = (ToolStripMenuItem)sender; //if (obj == null) return;
            if ((int)obj.Tag == 0) { //включить-выключить всплывающие подсказки
                GAME.ToolTipFlag = Enable_Visible_Tooltips_TSMI.Checked = !GAME.ToolTipFlag;
                for (int i = 0; i < tool_tip.Length; i++) if (tool_tip[i] != null) tool_tip[i].Active = !GAME.ToolTipFlag;
            } else if ((int)obj.Tag == 1) { //включить-выключить цветовую дифференциацию величин войск различных наций
                GAME.ColorArmyFlag = Enable_Visible_ColorArmy_TSMI.Checked = !GAME.ColorArmyFlag;
                if (Player.FolderAccount != "") Update_Panel_Army();
            }
            SetLanguage();//перевести всплывающие подсказки
            GAME.SaveInterface(GAME.PathFolderSave); 
        }

        /// <summary> Метод выбирает язык интерфейса. Ставит <b>Сhecked</b> напротив выбранного меню и переводит весь текст. </summary>
        private void SelectionOfLanguage_TSMI_Click(object sender, EventArgs e) {
            //ставим check у выбранного языка
            if (sender is ToolStripMenuItem TSMI) GAME.Language = (int)TSMI.Tag; else GAME.Language = (int)TSM1.DropDownItems[GAME.Language].Tag;
            for (int i = 0; i < TSM1.DropDownItems.Count; i++) (TSM1.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            (TSM1.DropDownItems[GAME.Language] as ToolStripMenuItem).Checked = true;
            LoadProcess.Languages = LANGUAGES;//нужно чтобы класс TLoadProcess выводил текст на свежевыбранном языке
            SetLanguage();//перевести весь текст
            GAME.SaveInterface(GAME.PathFolderSave);
        }

        /// <summary> Метод сохраняет игру в каждый файл. </summary>
        /// <remarks>
        ///     Аккаунт: <b>account.DAT, Interface.DAT, Village[N].DAT, Hero.DAT</b> <br/>
        ///     Карта: <b>map.DAT, cells.DAT</b> <br/>
        ///     Стек событий: <b>Event_Stack.DAT</b> <br/>
        ///     Отчёты: <b>Reports.DAT</b> <br/>
        ///     Сообщения: <b>Messages.DAT</b> <br/>
        ///     Альянсы: <b>Alliances.DAT</b> <br/>
        /// </remarks>
        private void SaveGameToolStripMenuItem_Click(object sender, EventArgs e) {
            if (BotList == null || BotList.Count == 0 || GAME.Map == null) { 
                MessageBox.Show("Error 20.\n" + LANGUAGES.RESOURSES[84]/*Сохранение невозможно.*/ + "\n" +
                                LANGUAGES.RESOURSES[85]/*Отсутствует список аккаунтов и/или карта.*/); return;
            } else if (Player.FolderAccount == "" || Player.FolderAccount == null) { 
                MessageBox.Show("Error 21.\n" + LANGUAGES.RESOURSES[84]/*Сохранение невозможно.*/ + "\n" +
                                LANGUAGES.RESOURSES[86]/*В аккаунте игрока не содержится имя папки для сохранения.*/); return; }

            //ставим игру на паузу, останавливаем потоки
            Event_Timer_Frontend.Stop(); Event_Thread_Timer_Backend.Change(0, Timeout.Infinite);

            GAME.ClearSaves(GAME.PathFolderSave);
            LoadProcess.Restart(LANGUAGES.RESOURSES[87]/*Сохранение. Аккаунт*/, BotList.Count);
            GAME.SaveInterface(GAME.PathFolderSave);//сохранение интерфейса
            Player.Hero.Name = "Лунтик";//тест удалить потом
            for (int i = 0; i < BotList.Count; i++) {
                BotList[i].SaveAccount($"{GAME.PathFolderSave}/{BotList[i].FolderAccount}");//сохранение аккаунта
                BotList[i].Hero.SaveHero($"{GAME.PathFolderSave}/{BotList[i].FolderAccount}");//сохранение героя
                string str = LANGUAGES.RESOURSES[87]/*Сохранение. Аккаунт*/ + $" [{i + 1}/{BotList.Count}] ";
                LoadProcess.LoadText.Text = str;
                for (int j = 0; j < BotList[i].VillageList.Count; j++) { //сохранение деревень и слотов аккаунта
                    LoadProcess.LoadText.Text = str + LANGUAGES.RESOURSES[18]/*Деревня*/ + $" {(j + 1)}/{BotList[i].VillageList.Count}.";
                    BotList[i].VillageList[j].SaveVillage($"{GAME.PathFolderSave}/{BotList[i].FolderAccount}");
                    for (int k = 0; k < BotList[i].VillageList[j].Slot.Length; k++) BotList[i].VillageList[j].Slot[k].SaveSlot($"{GAME.PathFolderSave}/{BotList[i].FolderAccount}", BotList[i].VillageList[j].Village_Name);
                    BotList[i].VillageList[j].Save_Reinforcements_Of_Troops($"{GAME.PathFolderSave}/{BotList[i].FolderAccount}");
                }
                LoadProcess._Update();
            }
            LoadProcess.Start(LANGUAGES.RESOURSES[110]/*Сохранение стека событий.*/, GAME.Event_Stack.Stack.Count);
                GAME.Event_Stack.SaveStack(LoadProcess, GAME.PathFolderSave);

            LoadProcess.Start(LANGUAGES.RESOURSES[138]/*Сохранение отчётов.*/, GAME.Reports.LIST.Count);
                GAME.Reports.SaveReports(LoadProcess, GAME.PathFolderSave);

            LoadProcess.Start(LANGUAGES.RESOURSES[140]/*Сохранение сообщений.*/, GAME.Messages.LIST.Count);
            GAME.Messages.SaveMessages(LoadProcess, GAME.PathFolderSave);

            LoadProcess.Start(LANGUAGES.RESOURSES[150]/*Сохранение альянсов.*/, GAME.Alliances.LIST.Count);
            GAME.Alliances.SaveAlliances(LoadProcess, GAME.PathFolderSave);

            LoadProcess.Start(LANGUAGES.RESOURSES[88]/*Сохранение карты.*/, GAME.Map.SizeMap());
                GAME.Map.SaveMap(GAME.PathFolderSave); GAME.Map.SaveCells(LoadProcess, GAME.PathFolderSave);
            LoadProcess.Stop(TLoadProcess.Report.Show);
            
            //снимаем игру с паузы, включаем потоки
            Event_Timer_Frontend.Start(); Event_Thread_Timer_Backend.Change(0, GAME.SpeedGame);
        }

        /// <summary> Метод вызывает окно "о программе". </summary>
        private void AboutTheProgram_TSMI_Click(object sender, EventArgs e) { WinDlg_AboutTheProgram(); }

        /// <summary> Метод закрывает игру. </summary>
        private void TSMI_Close_Click(object sender, EventArgs e) {
            //MessageBox.Show("Выход из программы TSMI_Close_Click()...");
            /*Close();*/ Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            //MessageBox.Show("Выход из программы Form1_FormClosing()...");
            /*Close();*/ Environment.Exit(0);
        }
        //====================================================== Menu Strip ======================================================
    }
}
