using Global_Var;
using System.Drawing;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static UFO.Convert;

namespace WindowsInterface
{
    interface IForm1_Interface {
        //ИНТЕРФЕЙС
        /// <summary> Метод обновляет интерфейс с текущими выбранными параметрами. <br/> Метод знает что и как обновлять. Достаточно просто вызвать и он всё сделает сам. </summary>
        void _Update();
        /// <summary> Метод обновляет всё ниже перечисленное. </summary>
        /// <value>
        /// <b> <paramref name="Parent"/>: </b> родительский Control для отображаемых панелей на нём (фон вкладки). <br/>
        /// <b> <paramref name="Panels_Map"/>: </b> обновляет всю информацию на вкладке "КАРТА / MAP". <br/>
        /// <b> <paramref name="Panel_Resources"/>: </b> обновляет панель ресурсов на складе новыми данными. <br/>
        /// <b> <paramref name="Panel_Resources_Production"/>: </b> обновляет панель добычи ресурсов новыми данными. <br/>
        /// <b> <paramref name="Panel_Village"/>: </b> обновляет панель информации о деревне. <br/>
        /// <b> <paramref name="Panel_VillageList"/>: </b> обновляет панель списка деревень. <br/>
        /// <b> <paramref name="Panel_Army"/>: </b> обновляет панель списка армии. <br/>
        /// <b> <paramref name="Picture_Ethnos"/>: </b> обновляет картинку нации выбранного/созданного аккаунта. <br/>
        /// <b> <paramref name="Buttons_Level_Slots"/>: </b> обновляет кнопки с текстом уровней построек в каждом слоте. <br/>
        /// <b> <paramref name="Panel_Troop_Movements"/>: </b> обновляет панель передвижений юнитов (войска, поселенцы, говоруны и т.д.). <br/>
        /// <b> <paramref name="Panel_Construction"/>: </b> обновляет панель строительства построек. <br/>
        /// <b> <paramref name="Panels_Statistics"/>: </b> обновляет информацию всех панелей на вкладке "СТАТИСТИКА / STATISTICS". <br/>
        /// <b> <paramref name="Panels_Report"/>: </b> обновляет информацию всех панелей на вкладке "ОТЧЁТЫ / REPORTS". <br/>
        /// <b> <paramref name="Panels_Message"/>: </b> обновляет информацию всех панелей на вкладке "СООБЩЕНИЯ / MESSAGES". <br/>
        /// <b> <paramref name="slots"/>: </b> определяет какие слоты будут обновляться в методе Update_Buttons_Level_Slots():
        ///                                    <b>Resourses:</b> ресурсные, <b>Village:</b> деревенские, или <b>None:</b> никакие. <br/>
        /// </value>
        void Update_Interface_State(Control Parent, bool Panels_Map = true, bool Panel_Resources = true,
                        bool Panel_Resources_Production = true, bool Panel_Village = true, bool Panel_VillageList = true, 
                        bool Panel_Army = true, bool Picture_Ethnos = true, bool Buttons_Level_Slots = true,
                        bool Panel_Troop_Movements = true, bool Panel_Construction = true, bool Panels_Statistics = true,
                        bool Panels_Report = true, bool Panels_Message = true,
                        Slots slots = Slots.Resources);
        /// <summary> Метод обновляет панель ресурсов на складе новыми данными. </summary>
        void Update_Panel_Resources();
        /// <summary> Метод обновляет панель передвижения войск новыми данными </summary>
        /// <remarks> 
        ///     Генерируются строки: <b> [ico] + >> + кол-во передвижений + текст + время прибытия </b> и лепятся друг за другом. <br/>
        ///     Габариты панели вычисляются в конце лепки.
        /// </remarks>
        void Update_Panel_Troop_Movements();
        /// <summary> Метод обновляет панель строительства построек в ресурсных полях и в деревне. </summary>
        void Update_Panel_Construction();
        /// <summary> Метод обновляет панель добычи ресурсов новыми данными. </summary>
        void Update_Panel_Resources_Production();
        /// <summary> Метод обновляет панель информации о деревне. </summary>
        void Update_Panel_VillageInfo();
        /// <summary> Метод обновляет панель списка деревень. <br/> Следует вызывать при загрузке/создании аккаунта, добавлении/удалении деревни (захват, заселение и т.д.) и переименовывании деревни. <br/> Во всех остальных случаях этот метод вызывать не следует. </summary>
        void Update_Panel_VillageList();
        /// <summary> Метод обновляет панель списка армии в выбранной деревне. </summary>
        void Update_Panel_Army();
        /// <summary> Метод обновляет картинку нации выбранного/созданного аккаунта. </summary>
        void Update_Picture_Ethnos();
        /// <summary> Метод обновляет кнопки с текстом уровней построек в каждом слоте выбранной (активной) деревни. </summary>
        void Update_Buttons_Level_Slots(Slots slots = Slots.Resources);
    }

    public partial class Form1 : Form, IForm1_Interface {
        //======================================================= ИНТЕРФЕЙС ======================================================
        public void _Update() {
            //вкладка 0. ГЛАВНОЕ МЕНЮ
            if (tabControl.SelectedIndex <= 0) return;
            //вкладка 1. ресурсные поля
            else if (tabControl.SelectedIndex == 1) Update_Interface_State(bg_Resources,
                                       Panels_Map: false, Panels_Statistics: false, Panels_Report: false);
            //вкладка 2. деревня с постройками
            else if (tabControl.SelectedIndex == 2)
                Update_Interface_State(bg_Village, Panels_Map: false, Panel_Army: false, Picture_Ethnos: false,
                                       Panel_Troop_Movements: false, Panels_Statistics: false, Panels_Report: false,
                                       slots: Slots.Village);
            //вкладка 3. карта
            else if (tabControl.SelectedIndex == 3)
                Update_Interface_State(bg_Map, true, true, false, false, true, false,
                                       false, false, false, false, false, false, false, Slots.None);
            //вкладка 4. статистика
            else if (tabControl.SelectedIndex == 4)
                Update_Interface_State(bg_Statistics, false, true, false, false, false, false,
                                       false, false, false, false, true, false, false);
            //вкладка 5. отчёты
            else if (tabControl.SelectedIndex == 5)
                Update_Interface_State(bg_Report, false, true, false, false, true, false,
                                       false, false, false, false, false, true, false);
            //вкладка 6. сообщения
            else if (tabControl.SelectedIndex == 6)
                Update_Interface_State(bg_Message, false, true, false, false, true, false,
                                       false, false, false, false, false, false, true);
        }

        public void Update_Interface_State(Control Parent, bool Panels_Map = true, bool Panel_Resources = true, 
                    bool Panel_Resources_Production = true, bool Panel_Village = true, bool Panel_VillageList = true,
                    bool Panel_Army = true, bool Picture_Ethnos = true, bool Buttons_Level_Slots = true, 
                    bool Panel_Troop_Movements = true, bool Panel_Construction = true, bool Panels_Statistics = true,
                    bool Panels_Report = true, bool Panels_Message = true,
                    Slots slots = Slots.Resources) {
            //ВОЗМОЖНО РАЗДЕЛИТЬ Parent и отрисовку чтобы меньше панелей перерисовывать за зря
            if (Panel_Resources) { this.Panel_Resources.Parent = Parent; Update_Panel_Resources(); }//обновляет панель ресурсов на складе новыми данными
            if (Panel_Resources_Production) { this.Panel_Resources_Production.Parent = grid_Resources_Production.Parent = Parent; 
                Update_Panel_Resources_Production(); }//обновляет панель добычи ресурсов новыми данными
            if (Panel_Village) { Panel_Village1.Parent = Panel_Village2.Parent = Parent; Update_Panel_VillageInfo(); }//обновляет панель информации о деревне
            if (Panel_VillageList) { GroupBox_Village.Parent = Parent; Update_Panel_VillageList(); }//обновляет панель списка деревень
            if (Panel_Army) { this.Panel_Army.Parent = grid_Army.Parent = Parent; Update_Panel_Army(); }//обновляет панель списка армии в выбранной деревне
            if (Picture_Ethnos) { this.Picture_Ethnos.Parent = lb_Ethnos.Parent = Parent; Update_Picture_Ethnos(); } //обновляет картинку нации выбранного/созданного аккаунта
            if (Buttons_Level_Slots) Update_Buttons_Level_Slots(slots);//обновляет кнопки с текстом уровней построек в каждом слоте
            if (Panel_Troop_Movements) { Panel_Move_Units.Parent = grid_Move_Units.Parent = Parent; 
                Update_Panel_Troop_Movements(); }//обновляет панель передвижений юнитов (войск, поселенцев, говорунов и т.д.)
            if (Panel_Construction) { this.Panel_Construction.Parent = Parent; Update_Panel_Construction(); }//обновляет панель строительства построек в ресурсных полях и в деревне
            if (Panels_Map) Update_Panels_Map();//обновляет всю информацию на вкладке "КАРТА / MAP"
            if (Panels_Statistics) { 
                if ((int)btn_Statistics_Players.Tag == 1) Update_Panels_Statistics(TypeStatistics.Players);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
                else if ((int)btn_Statistics_Alliances.Tag == 1) Update_Panels_Statistics(TypeStatistics.Alliances);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
                else if ((int)btn_Statistics_Villages.Tag == 1) Update_Panels_Statistics(TypeStatistics.Villages);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
                else if ((int)btn_Statistics_Heroes.Tag == 1) Update_Panels_Statistics(TypeStatistics.Heroes);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
                else if ((int)btn_Statistics_Wonders.Tag == 1) Update_Panels_Statistics(TypeStatistics.Wonders);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
                else Update_Panels_Statistics(TypeStatistics.None);//обновляет информацию на вкладке "СТАТИСТИКА / STATISTICS"
            }
            if (Panels_Report) Update_Panels_Reports();//обновляет информацию на вкладке "ОТЧЁТЫ / REPORTS"
            if (Panels_Message) Update_Panels_Messages();//обновляет информацию на вкладке "СООБЩЕНИЯ / MESSAGES"
            //перецепляем Parent кнопок в шапке
            pb_Button_Village.Parent = pb_Button_Resourses.Parent = pb_Button_Map.Parent = 
            pb_Button_Statistics.Parent = pb_Button_Reports.Parent = pb_Button_Messages.Parent = Parent;
            //перецепляем Parent кнопок переключения скорости игры
            btn_SpeedGame1.Parent = btn_SpeedGame2.Parent = btn_SpeedGame3.Parent = 
            btn_SpeedGame4.Parent = btn_SpeedGame5.Parent = btn_SpeedGame6.Parent = Parent;
            btn_Hero.Parent = Parent;//кнопка героя на портрете
        }
        /// <summary> Метод обрабатывает выбор деревни из списка деревень. </summary>
        private void ListBox_VillageList1_MouseUp(int i = 0) {
            int newIndex = ListBox1.SelectedIndex + i; if (newIndex < 0 || newIndex >= ListBox1.Items.Count) return;
            Player.ActiveIndex_set_Village(ListBox1.Items[newIndex].ToString());
            ListBox2.SelectedIndex = newIndex;
            GAME.Map.Location = Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian;
            BackGroundResources();
            _Update();
        }
        /// <summary> Метод обрабатывает выбор деревни из списка координат деревень. </summary>
        private void ListBox_VillageList2_MouseUp(int i = 0) {
            int newIndex = ListBox2.SelectedIndex + i; if (newIndex < 0 || newIndex >= ListBox2.Items.Count) return;
            Player.ActiveIndex_set_Коры(ListBox2.Items[newIndex].ToString());
            ListBox1.SelectedIndex = newIndex;
            GAME.Map.Location = Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian;
            BackGroundResources();
            _Update();

            //преобразовываем координаты аккаунта с (0, 0) в координаты карты Map.Cell[x][y] и вызываем окно свойств деревни
            GAME.Map.Location = new Point(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.X, Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.Y);
            Point p = GAME.Map.Cell_to_Tor(GAME.Map.Location.X + GAME.Map.Width, GAME.Map.Location.Y + GAME.Map.Height);
            winDlg_InfoCell(p.X, p.Y);
        }
        private void ListBox_VillageList_MouseUp(object sender, MouseEventArgs e) { ListBox_VillageList1_MouseUp(); }//мышка
        private void ListBox_VillageList2_MouseUp(object sender, MouseEventArgs e) { ListBox_VillageList2_MouseUp(); }//мышка
        private void ListBox1_KeyUp(object sender, KeyEventArgs e) {    //клавиатура
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) ListBox_VillageList1_MouseUp();
            else if (e.KeyCode == Keys.W) ListBox_VillageList1_MouseUp(-1); else if (e.KeyCode == Keys.S) ListBox_VillageList1_MouseUp(1);
        }
        private void ListBox2_KeyUp(object sender, KeyEventArgs e) {    //клавиатура
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) ListBox_VillageList2_MouseUp();
            else if (e.KeyCode == Keys.W) ListBox_VillageList2_MouseUp(-1); else if (e.KeyCode == Keys.S) ListBox_VillageList2_MouseUp(1);
        }

        /// <summary> Метод красит ProgressBar-ы панели ресурсов в хранилищах в красный цвет (вызывается при переполнении). </summary>
        private void FullStackResource(Button btn, ProgressBar pb) {
            btn.Left = 0; btn.Top = 0; btn.Width = pb.Width; btn.Height = pb.Height; btn.Parent = pb;
            btn.Button_Styles(Color.Firebrick, Color.Firebrick, Color.Firebrick, Color.Maroon, Color.Maroon, 3); btn.Visible = true;
        }

        public void Update_Panel_Resources() {
            if (Player.VillageList.Count <= 0) return;
            int Sklad = Player.VillageList[Player.ActiveIndex].Capacity.Warehouse;
            int Ambar = Player.VillageList[Player.ActiveIndex].Capacity.Barn;
            int Kazna = Player.VillageList[Player.ActiveIndex].Capacity.Treasury;
            int Crop_in_chas = (int)Player.VillageList[Player.ActiveIndex].HourlyProductionResources.crop;

            int u_wood = (int)Player.VillageList[Player.ActiveIndex].ResourcesInStorages.wood;
            string s_wood = toTABString(u_wood.ToString(), "'") + "/" + toTABString(Sklad.ToString(), "'");
            int u_clay = (int)Player.VillageList[Player.ActiveIndex].ResourcesInStorages.clay;
            string s_clay = toTABString(u_clay.ToString(), "'") + "/" + toTABString(Sklad.ToString(), "'");
            int u_iron = (int)Player.VillageList[Player.ActiveIndex].ResourcesInStorages.iron;
            string s_iron = toTABString(u_iron.ToString(), "'") + "/" + toTABString(Sklad.ToString(), "'");
            int u_crop = (int)Player.VillageList[Player.ActiveIndex].ResourcesInStorages.crop;
            string s_crop = toTABString(u_crop.ToString(), "'") + "/" + toTABString(Ambar.ToString(), "'");
            int u_cons_crop = Player.VillageList[Player.ActiveIndex].Crop_Consumption;
            string s_cons_crop = toTABString(u_cons_crop.ToString(), "'") + "/" + toTABString(Crop_in_chas.ToString(), "'");
            int u_gold = (int)Player.VillageList[Player.ActiveIndex].ResourcesInStorages.gold;
            string s_gold = toTABString(u_gold.ToString(), "'") + "/" + toTABString(Kazna.ToString(), "'");

            int tab = ToCSR(20);/*отступ между блоками*/
            //дерево
            lb_wood.Text = s_wood; pic_wood.Left = tab; lb_wood.Left = pic_wood.Right;
            pb_wood.Left = pic_wood.Left; pb_wood.Top = lb_wood.Bottom + 5;
            pb_wood.Width = pic_wood.Width + lb_wood.Width;
            pb_wood.Maximum = Sklad; 
            if (u_wood >= 0) pb_wood.Value = u_wood; else pb_wood.Value = 0;
            if (u_wood >= Sklad) FullStackResource(AlternativeProgress_wood, pb_wood);//переполнение хранилища
            else AlternativeProgress_wood.Visible = false;
            //глина
            lb_clay.Text = s_clay; pic_clay.Left = lb_wood.Right + tab;
            lb_clay.Left = pic_clay.Right; pb_clay.Left = pic_clay.Left; pb_clay.Top = lb_clay.Bottom + 5;
            pb_clay.Width = pic_clay.Width + lb_clay.Width;
            pb_clay.Maximum = Sklad; pb_clay.Value = u_clay;
            if (u_clay >= 0) pb_clay.Value = u_clay; else pb_clay.Value = 0;
            if (u_clay >= Sklad) FullStackResource(AlternativeProgress_clay, pb_clay);//переполнение хранилища
            else AlternativeProgress_clay.Visible = false;
            //железо
            lb_iron.Text = s_iron; pic_iron.Left = lb_clay.Right + tab;
            lb_iron.Left = pic_iron.Right; pb_iron.Left = pic_iron.Left; pb_iron.Top = lb_iron.Bottom + 5;
            pb_iron.Width = pic_iron.Width + lb_iron.Width;
            pb_iron.Maximum = Sklad; 
            if (u_iron >= 0) pb_iron.Value = u_iron; else pb_iron.Value = 0;
            if (u_iron >= Sklad) FullStackResource(AlternativeProgress_iron, pb_iron);//переполнение хранилища
            else AlternativeProgress_iron.Visible = false;
            //зерно
            lb_crop.Text = s_crop; pic_crop.Left = lb_iron.Right + tab;
            lb_crop.Left = pic_crop.Right; pb_crop.Left = pic_crop.Left; pb_crop.Top = lb_crop.Bottom + 5;
            pb_crop.Width = pic_crop.Width + lb_crop.Width;
            pb_crop.Maximum = Ambar;
            if (u_crop >= 0) pb_crop.Value = u_crop; else pb_crop.Value = 0;
            if (u_crop >= Ambar) FullStackResource(AlternativeProgress_crop, pb_crop);//переполнение хранилища
            else AlternativeProgress_crop.Visible = false;
            //потребление зерна
            lb_cons_crop.Text = s_cons_crop; pic_cons_crop.Left = lb_crop.Right + tab;
            lb_cons_crop.Left = pic_cons_crop.Right; pb_cons_crop.Left = pic_cons_crop.Left; pb_cons_crop.Top = lb_cons_crop.Bottom + 5;
            pb_cons_crop.Width = pic_cons_crop.Width + lb_cons_crop.Width;
            pb_cons_crop.Maximum = Crop_in_chas;
            if (u_cons_crop <= pb_cons_crop.Value) pb_cons_crop.Value = u_cons_crop; else pb_cons_crop.Value = pb_cons_crop.Maximum;
            if (u_cons_crop >= Crop_in_chas) FullStackResource(AlternativeProgress_cons_crop, pb_cons_crop);//переполнение хранилища
            else AlternativeProgress_cons_crop.Visible = false;
            //золото
            lb_gold.Text = s_gold; pic_gold.Left = lb_cons_crop.Right + tab;
            lb_gold.Left = pic_gold.Right; pb_gold.Left = pic_gold.Left; pb_gold.Top = lb_gold.Bottom + 5;
            pb_gold.Width = pic_gold.Width + lb_gold.Width;
            pb_gold.Maximum = Kazna;
            if (u_gold >= 0) pb_gold.Value = u_gold; else pb_gold.Value = 0;
            if (u_gold >= Kazna) FullStackResource(AlternativeProgress_gold, pb_gold);//переполнение хранилища
            else AlternativeProgress_gold.Visible = false;

            Panel_Resources.Left = (tabControl.Width - Panel_Resources.Width) / 2;
            Panel_Resources.Top = pb_Button_Resourses.Bottom;
        }

        public void Update_Panel_Troop_Movements() {
            //преобразовываем координаты аккаунта с (0, 0) в координаты карты Map.Cell[x][y]
            Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);

            if (GAME.Event_Stack.IsMove(p)) {    //передвижения есть
                int count_line = 6;//максимальное кол-во строк на панели
                int[] count = new int[6]; int[] time = new int[6];
                time[0] = time[1] = time[2] = time[3] = time[4] = time[5] = int.MaxValue;
                //считаем количество перемещений юнитов в выбранных координатах и ближайшее время прибытия
                for (int i = 0; i < GAME.Event_Stack.Stack.Count; i++) { var Stack = GAME.Event_Stack.Stack[i];
                    //RED_attack
                    if ((Stack.TypeEvent == Type_Event.ATTACK || Stack.TypeEvent == Type_Event.RAID) 
                        && Stack.Cell_Finish == p) { count[0]++; if (Stack.timer < time[0]) time[0] = Stack.timer; }
                    //GREEN_shield
                    if (Stack.TypeEvent == Type_Event.REINFORCEMENTS && Stack.Cell_Finish == p) {
                        count[1]++; if (Stack.timer < time[1]) time[1] = Stack.timer; }
                    //YELLOW_attack
                    if ((Stack.TypeEvent == Type_Event.ATTACK || Stack.TypeEvent == Type_Event.RAID)
                        && Stack.Cell_Start == p) { count[2]++; if (Stack.timer < time[2]) time[2] = Stack.timer; }
                    //YELLOW_shield
                    if (Stack.TypeEvent == Type_Event.REINFORCEMENTS && Stack.Cell_Start == p) {
                        count[3]++; if (Stack.timer < time[3]) time[3] = Stack.timer; }
                    //PURPLE_attack
                    if ((Stack.TypeEvent == Type_Event.ADVENTURE_ATTACK || Stack.TypeEvent == Type_Event.ADVENTURE_RAID)
                        && Stack.Cell_Start == p) { count[4]++; if (Stack.timer < time[4]) time[4] = Stack.timer; }
                    //send_settlers
                    if (Stack.TypeEvent == Type_Event.ESTABLISH_A_SETTLEMENT && Stack.Cell_Start == p) {
                        count[5]++; if (Stack.timer < time[5]) time[5] = Stack.timer; }
                }

                Image[] ICO = new Image[6];
                ICO[0] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/RED_attacker" + N_pic + ".png");
                ICO[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/won_defender" + N_pic + ".png");
                ICO[2] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/YELLOW_attacker" + N_pic + ".png");
                ICO[3] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/won_defender" + (N_pic + 2) + ".png");
                ICO[4] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/PURPLE_attacker" + N_pic + ".png");
                ICO[5] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/settlers" + N_pic + ".png");
                string[] type_move = new string[6];
                type_move[0] = LANGUAGES.RESOURSES[92];/*Нападений*/ type_move[1] = LANGUAGES.RESOURSES[93];/*Подкреп.*/
                type_move[2] = LANGUAGES.RESOURSES[92];/*Нападений*/ type_move[3] = LANGUAGES.RESOURSES[93];/*Подкреп.*/
                type_move[4] = LANGUAGES.RESOURSES[92];/*Нападений*/ type_move[5] = LANGUAGES.RESOURSES[94];/*Расселение*/

                grid_Move_Units.Columns[1].DefaultCellStyle.Font = new Font(grid_Move_Units.Font.FontFamily, lb_Move_Units_Header.Font.Size - 2, FontStyle.Bold);
                grid_Move_Units.Rows.Clear();
                for (int i = 0; i < count_line; i++) if (count[i] > 0) {
                    grid_Move_Units.Rows.Add(ICO[i], "« " + count[i] + " " + type_move[i] + " ",
                        ToTimeString(time[i]) + " " + LANGUAGES.Time[0] + "."
                    );
                    //отображаем цвет меток
                    var Cell = grid_Move_Units.Rows[grid_Move_Units.Rows.Count - 1].Cells[1];
                    if (i == 0) Cell.Style.ForeColor = Color.FromArgb(220, 0, 0);//красный
                    if (i == 1) Cell.Style.ForeColor = Color.FromArgb(60, 130, 40);//зелёный
                    if (i == 2) Cell.Style.ForeColor = Color.FromArgb(225, 200, 0);//жёлтый
                    if (i == 3) Cell.Style.ForeColor = Color.FromArgb(225, 200, 0);//жёлтый
                    if (i == 4) Cell.Style.ForeColor = Color.FromArgb(190, 65, 190);//пурпурный
                    if (i == 5) Cell.Style.ForeColor = Color.FromArgb(60, 130, 40);//зелёный
                }
            } else {    //передвижений нет
                grid_Move_Units.Columns[1].DefaultCellStyle.Font = new Font(grid_Move_Units.Font.FontFamily, lb_Move_Units_Header.Font.Size - 2, FontStyle.Regular);
                grid_Move_Units.Columns[1].DefaultCellStyle.ForeColor = Color.Black;
                grid_Move_Units.Rows.Clear(); grid_Move_Units.Rows.Add(new Bitmap(1, 1), LANGUAGES.RESOURSES[55]/*Нет*/, "");
            }

            //подгон габаритов Grid (убираем горизонтальную/вертикальную полосы прокрутки)
            grid_Move_Units.Width = 0; for (int i = 0; i < grid_Move_Units.Columns.Count; i++) grid_Move_Units.Width += grid_Move_Units.Columns[i].Width;
            grid_Move_Units.Height = grid_Move_Units.Rows.Count * grid_Move_Units.Rows[0].Height + ToCSR(10);
            //выравниваем эту панель под ширину панели добычи ресурсов
            if (grid_Move_Units.Width < Panel_Resources_Production.Width) grid_Move_Units.Width = Panel_Resources_Production.Width;
            Panel_Move_Units.Width = grid_Move_Units.Width;//выравниваем ширину панели под таблицу
            //лепим по центру между строкой-складом и панелью добычи ресурсов
            int summH = Panel_Move_Units.Height + grid_Move_Units.Height;
            int h = ((Panel_Resources_Production.Top - Panel_Resources.Bottom) - summH) / 2;
            Panel_Move_Units.Top = Panel_Resources.Bottom + h;
            grid_Move_Units.Location = new Point(Panel_Move_Units.Left, Panel_Move_Units.Bottom);//прикрепляем таблицу к панели
            grid_Move_Units.ClearSelection(); grid_Move_Units.Enabled = false;
        }

        /// <summary> Массив картинок-иконок отображающих пиктограмму отмены строительства. </summary>
        private PictureBox[] ico2 = null;
        /// <summary> Массив меток с информацией о названии постройки. </summary>
        public Label[] Building_Name = null;
        /// <summary> Массив меток с информацией об уровне строящейся постройки. </summary>
        public Label[] Building_lvl = null;
        /// <summary> Массив меток с информацией об оставшемся времени до завершения строительства. </summary>
        public Label[] Building_Time = null;
        public void Update_Panel_Construction() {
            //преобразовываем координаты аккаунта с (0, 0) в координаты карты Map.Cell[x][y]
            Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);

            if (GAME.Event_Stack.IsConstruction(p)) {  //есть строительство
                Panel_Construction.Visible = true;
                int count_line = GAME.Get_MaxCount_Building_InOfConstructionQueue(Player.Folk_Name);//максимальное кол-во строк на панели

                //изменяем длину массивов т.к. во время запущенной программы был создан аккаунт ещё раз
                //с другим кол-вом одновременных построек
                if (ico2 != null && ico2.Length != count_line) { for (int i = 0; i < ico2.Length; i++) {
                    ico2[i].Dispose(); Building_Name[i].Dispose(); Building_lvl[i].Dispose();
                    Building_Time[i].Dispose();
                } ico2 = null; Building_Name = null; Building_lvl = null; Building_Time = null; }

                //создаём визуальные контролы
                if (ico2 == null) {
                    ico2 = new PictureBox[count_line];       Building_Name = new Label[count_line];
                    Building_lvl = new Label[count_line];    Building_Time = new Label[count_line];
                    for (int i = 0; i < ico2.Length; i++) {
                        Building_Name[i] = new Label {
                            Parent = Panel_Construction, AutoSize = true, ForeColor = Color.Black, Text = "null",
                            Font = new Font(lb_Construction_Header.Font.FontFamily, ToCSR(10), FontStyle.Regular) };
                        Building_lvl[i] = new Label {
                            Parent = Panel_Construction, AutoSize = true, ForeColor = Color.FromArgb(255, 120, 0), Text = "null",
                            Font = new Font(lb_Construction_Header.Font.FontFamily, ToCSR(10), FontStyle.Bold) };
                        Building_Time[i] = new Label {
                            Parent = Panel_Construction, AutoSize = true, ForeColor = Color.Black, Text = "null",
                            Font = new Font(lb_Construction_Header.Font.FontFamily, ToCSR(10), FontStyle.Regular) };
                        ico2[i] = new PictureBox {
                            Parent = Panel_Construction, Name = "ico2", BackgroundImageLayout = ImageLayout.Stretch,
                            Size = new Size(ToCSR(12), ToCSR(12)),
                            BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/del.png"),//красный крестик
                        }; ico2[i].Click += Control_Click;
                }}

                //заполняем метки текстом о строительстве из стека событий
                int N = 0;
                for (int i = 0; i < GAME.Event_Stack.Stack.Count; i++) {
                    if (GAME.Event_Stack.Stack[i].TypeEvent == Type_Event.Construction &&
                        GAME.Event_Stack.Stack[i].Cell_Start == p) {
                        if (N < ico2.Length) {
                            ico2[N].Tag = GAME.Event_Stack.Stack[i];//Tag ссылка на строку Stack[]
                            if (N - 1 >= 0) ico2[N - 1].Tag = null;//предыдущий Tag зануляем, нужен только последний
                            string str_Снос = ""; if (GAME.Event_Stack.Stack[i].Destruction) str_Снос = LANGUAGES.RESOURSES[112];/*(cнос)*/
                            Building_Name[N].Text = LANGUAGES.buildings[(int)GAME.Event_Stack.Stack[i].ID] + " " + str_Снос;
                            Building_lvl[N].Text = LANGUAGES.RESOURSES[3]/*Level*/ + " " + GAME.Event_Stack.Stack[i].lvl.ToString();
                            Building_Time[N].Text = LANGUAGES.RESOURSES[98]/*Готово через*/ + " " + ToTimeString(GAME.Event_Stack.Stack[i].timer) + " " + LANGUAGES.Time[0]/*ч.*/ + ".";
                            //ставим флаги у тех строк которые будем отображать на панели
                            ico2[N].Visible = Building_Name[N].Visible = Building_lvl[N].Visible = Building_Time[N].Visible = true;
                        } N++;
                    }
                }
                for (; N < ico2.Length; N++) { //остальные строки не рисуем
                    ico2[N].Visible = Building_Name[N].Visible = Building_lvl[N].Visible = Building_Time[N].Visible = false;
                }

                //вычисляем положение меток и картинок на панели Panel_Construction
                int x = ToCSR(20), y = btn_Construction.Bottom + ToCSR(10);
                int max_Width = 0; int h = 0;
                for (int i = 0; i < ico2.Length; i++) if (ico2[i].Visible) { 
                    ico2[i].Location = new Point(x, y);
                    Building_Name[i].Location = new Point(x + ico2[i].Width + ToCSR(5), y - (ico2[i].Height / 3));
                    Building_lvl[i].Location = new Point(Building_Name[i].Right + ToCSR(5), Building_Name[i].Top);
                    y += ico2[i].Height + ToCSR(10); if (Building_lvl[i].Right > max_Width) max_Width = Building_lvl[i].Right;
                }
                for (int i = 0; i < ico2.Length; i++) if (ico2[i].Visible) {
                        Building_Time[i].Location = new Point(max_Width + ToCSR(100), Building_Name[i].Top);
                        h = Building_Time[i].Bottom;
                }
                btn_Construction.Left = Building_Time[0].Right - btn_Construction.Width;
            } else { Panel_Construction.Visible = false; }//строительства нет
        }

        public void Update_Panel_Resources_Production() {
            lb_Resources_Production_Header.Text = LANGUAGES.RESOURSES[53];/*Производство в час:*/
            var HPR = Player.VillageList[Player.ActiveIndex].HourlyProductionResources;
            var POI = Player.VillageList[Player.ActiveIndex].HourlyProductionResources_PercentOfIncrease;
            double[] res = new double[5]; res[0] = HPR.wood; res[1] = HPR.clay; res[2] = HPR.iron; res[3] = HPR.crop; res[4] = HPR.gold;
            double[] percent = new double[5]; percent[0] = POI.wood; percent[1] = POI.clay; percent[2] = POI.iron; percent[3] = POI.crop; percent[4] = POI.gold;
            string[] NameRes = new string[5];
            NameRes[0] = LANGUAGES.RESOURSES[10];/*Древесина:*/ NameRes[1] = LANGUAGES.RESOURSES[12];/*Глина:*/
            NameRes[2] = LANGUAGES.RESOURSES[13];/*Железо:*/    NameRes[3] = LANGUAGES.RESOURSES[11];/*Зерно:*/
            NameRes[4] = LANGUAGES.RESOURSES[41];/*Золото:*/
            //добавляем строки с выработкой ресурсов
            grid_Resources_Production.Rows.Clear(); grid_Resources_Production.Rows.Add(new Bitmap(1, 1), "", "", "");
            for (int i = 0; i < 5; i++) { int j; if (i == 4) j = 5; else j = i; int value; string sign = "";
                if (i == 3) value = (int)(res[i] + (res[i] / 100 * percent[i]) - Player.VillageList[Player.ActiveIndex].Crop_Consumption);
                else value = (int)(res[i] + (res[i] / 100 * percent[i])); if (value < 0) sign = "-";
                grid_Resources_Production.Rows.Add(Image.FromFile("DATA_BASE/IMG/pictograms/resources/0" + j + ".png"),
                    " " + NameRes[i] + " ", sign + toTABString(value.ToString()), " " + LANGUAGES.RESOURSES[52]/*в час*/);
                Color cl; if (value < 0) cl = Color.Red; else cl = Color.Black; grid_Resources_Production.Rows[grid_Resources_Production.Rows.Count - 1].Cells[2].Style.ForeColor = cl;
            }
            //Если панель добычи ресурсов уехала за границу экрана, корректируем Left
            if (tabControl.SelectedIndex == 2) Panel_Resources_Production.Left = Global.MainWindowWidth - Panel_Resources_Production.Width - ToCSR(20);
            //подгон ширины Grid (убираем горизонтальную полосу прокрутки)
            grid_Resources_Production.Width = grid_Resources_Production.RowHeadersWidth; for (int i = 0; i < grid_Resources_Production.Columns.Count; i++) grid_Resources_Production.Width += grid_Resources_Production.Columns[i].Width;
            grid_Resources_Production.Location = new Point(Panel_Resources_Production.Left, Panel_Resources_Production.Bottom);
            Panel_Resources_Production.Width = grid_Resources_Production.Width;
            lb_Resources_Production_Header.Centering();
            //денамическое изменение размера панели с войсками в зависимости от её высоты
            grid_Resources_Production.Height = grid_Resources_Production.ColumnHeadersHeight + grid_Resources_Production.Rows.Count * grid_Resources_Production.Rows[0].Height + ToCSR(10);
            grid_Resources_Production.ClearSelection(); grid_Resources_Production.Enabled = false;
        }

        public void Update_Panel_VillageInfo() {
            if (Player.VillageList.Count <= 0) return;
            lb_Village1.ForeColor = Color.DarkRed; lb_Village2.ForeColor = Color.Blue; lb_Village3.ForeColor = Color.FromArgb(128, 128, 128);
            lb_Village4.ForeColor = lb_Village5.ForeColor = Color.FromArgb(94, 70, 58);
            //Панель 1
            lb_Village1.Text = Player.Nick_Name;
            lb_Village2.Text = Player.VillageList[Player.ActiveIndex].Village_Name;
            if (Player.ActiveIndex == Player.NumberOfCapital) lb_Village3.Text = LANGUAGES.RESOURSES[42] + " ";/*(Столица)*/ else lb_Village3.Text = "";
            lb_Village3.Text += LANGUAGES.RESOURSES[43]/*Одобрение*/ + " " + Player.VillageList[Player.ActiveIndex].Approval.ToString() + "%";

            //Панель 2
            lb_Village4.Text = LANGUAGES.RESOURSES[44]/*Население деревни:*/ + " " + toTABString(Player.VillageList[Player.ActiveIndex].Population.ToString(), "'");
            lb_Village5.Text = LANGUAGES.RESOURSES[45]/*Общее население:*/ + " " + toTABString(Player.Total_Population.ToString(), "'");

            if (tabControl.SelectedIndex >= 2) { //деревня
                Panel_Village1.BackgroundImage = Panel_Village2.BackgroundImage = null;
                Panel_Village1.BackColor = Panel_Village2.BackColor = Color.FromArgb(239, 227, 202);//фон
                Panel_Village1.Contour_Solid(3, Color.FromArgb(165, 145, 105));//рамка
                Panel_Village2.Contour_Solid(3, Color.FromArgb(165, 145, 105));//рамка
            }
            else if (tabControl.SelectedIndex == 1) { //ресурсные поля
                Panel_Village1.BackgroundImage = Panel_Village2.BackgroundImage = null;
                Panel_Village1.BackColor = Panel_Village2.BackColor = Color.Transparent;
            }
        }

        public void Update_Panel_VillageList() {
            ListBox1.BeginUpdate(); ListBox2.BeginUpdate();//выключить отрисовку компонента на момент добавления чтобы не тормозило при 125 деревнях
            GroupBox_Village.SizeFont(14F, FontStyle.Bold); ListBox1.SizeFont(10F, FontStyle.Bold); ListBox2.Font = ListBox1.Font;
            ListBox1.Items.Clear(); string[] tmp = new string[Player.VillageList.Count];
            for (int i = 0; i < tmp.Length; i++) { tmp[i] = Player.VillageList[i].Village_Name; }
            ListBox1.Items.AddRange(tmp);/*Village*/

            ListBox2.Items.Clear(); string[] tmp2 = new string[Player.VillageList.Count];
            for (int i = 0; i < tmp.Length; i++) for (int j = 0; j < tmp.Length; j++) if (ListBox1.Items[i].ToString() == Player.VillageList[j].Village_Name) {
                tmp2[i] = "(" + Player.VillageList[j].Coordinates_World_Travian.X.ToString() + "|" + Player.VillageList[j].Coordinates_World_Travian.Y.ToString() + ")"; break;
            }
            ListBox2.Items.AddRange(tmp2);/*координаты Village*/

            GroupBox_Village.Text = LANGUAGES.RESOURSES[46]/*Деревни*/ + " " + Player.VillageList.Count + "/" + Player.Limit_Village;
            ListBox1.Size = ListBox1.GetPreferredSize(ListBox1.Size); ListBox2.Size = ListBox2.GetPreferredSize(ListBox2.Size);
            GroupBox_Village.Width = 5 + ListBox1.Width + 5 + ListBox2.Width + 5;

            //позиция и размер кнопки "переименовать деревню" на GroupBox_Village
            btn_Rename_Village.Location = new Point(GroupBox_Village.Width - btn_Rename_Village.Width - 5, 5); //btn_Rename_Village.Size(25, 25); 
            //выравниваем списки по центру GroupBox_Village
            int width = (GroupBox_Village.Width - ListBox1.Width - ListBox2.Width - 5) / 2;
            ListBox1.Left = width; ListBox2.Left = width + ListBox1.Width + 5;

            GroupBox_Village.BackColor = Color.FromArgb(239, 227, 202);//фон
            ListBox1.BackColor = ListBox2.BackColor = Color.FromArgb(239, 227, 202);//фон

            ListBox1.EndUpdate(); ListBox2.EndUpdate();//раздупляем компонент обратно. это можно делать при например если tmp.length > 10
            //выделяем строку
            for (int i = 0; i < ListBox1.Items.Count; i++) {
                if (ListBox1.Items[i].ToString() == Player.VillageList[Player.ActiveIndex].Village_Name)
                    { ListBox1.SelectedIndex = ListBox2.SelectedIndex = i; }
            }

            //динамическое изменение высоты панели в зависимости от её свойства Top
            int Height = ListBox1.Bottom + ToCSR(10);
            if (Height <= GroupBox_Village.Parent.Height - GroupBox_Village.Top - ToCSR(40)) GroupBox_Village.Height = Height;
            else GroupBox_Village.Height = GroupBox_Village.Parent.Height - GroupBox_Village.Top - ToCSR(40);
            ListBox1.Height = ListBox2.Height = GroupBox_Village.Height - ListBox1.Top - ToCSR(10);
        }

        /// <summary> Поле запоминает текущую позицию полосы прокрутки в таблице армии. </summary>
        private int FDSRI = 0;
        public void Update_Panel_Army() {
            grid_Army.Visible = false;//скрываем таблицу на время отрисовки, чтобы меньше дёргалась
            grid_Army.Rows.Clear(); 
            lb_Army_Header.Text = LANGUAGES.RESOURSES[54];/*Войска:*/    
            int cx = GAME.Map.Width + Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.X;
            int cy = GAME.Map.Height + Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.Y;
            int TroopCount = 0; for(int i = 0; i < GAME.Map.Cell[cx, cy].AllTroops.Length; i++) if (GAME.Map.Cell[cx, cy].AllTroops[i] > 0) TroopCount++;
            if (Player.Hero.InArmy(Player.VillageList[Player.ActiveIndex].Village_Name)) TroopCount++;

            if (TroopCount <= 0) { //в деревне войск нет
                grid_Army.Columns[1].DefaultCellStyle.Font = new Font(grid_Army.Font.FontFamily, lb_Army_Header.Font.Size - 1, FontStyle.Regular);
                grid_Army.Columns[1].DefaultCellStyle.ForeColor = Color.Black;
                grid_Army.Enabled = false; grid_Army.Rows.Add(new Bitmap(1, 1), "", "");
                grid_Army.Rows.Add(new Bitmap(1, 1), LANGUAGES.RESOURSES[55]/*Нет*/, " ");
                grid_Army.Rows.Add(new Bitmap(1, 1), "", "");
            }
            else {                 //в деревне войска есть
                grid_Army.Enabled = true;
                grid_Army.Columns[1].DefaultCellStyle.Font = new Font(grid_Army.Font.FontFamily, lb_Army_Header.Font.Size - 2, FontStyle.Bold);

                string Hero_str = LANGUAGES.RESOURSES[141];/*Герой*/;
                //добавление героя игрока если он есть
                if (Player.Hero.InArmy(Player.VillageList[Player.ActiveIndex].Village_Name)) {
                    grid_Army.Rows.Add(Image.FromFile("DATA_BASE/IMG/pictograms/unit/Hero/51.png"), 1, Hero_str);
                    grid_Army.Rows[grid_Army.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Black;
                }
                //добавление героев из подкрепов если они есть
                if (GAME.Map.Cell[cx, cy].AllTroops[50] > 0) {
                    grid_Army.Rows.Add(Image.FromFile("DATA_BASE/IMG/pictograms/unit/Hero/51.gif"),
                        GAME.Map.Cell[cx, cy].AllTroops[50], Hero_str);
                    grid_Army.Rows[grid_Army.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Black;
                }

                for (int i = 0; i < GAME.Map.Cell[cx, cy].AllTroops.Length - 1; i++) { //добавление строк
                    if (GAME.Map.Cell[cx, cy].AllTroops[i] > 0) {
                        string path = "DATA_BASE/IMG/pictograms/unit/"; string path2 = "Rome/"; string extension = ".gif";
                        if (i / 10 == 0) path2 = "Rome/"; else if (i / 10 == 1) path2 = "German/"; else
                        if (i / 10 == 2) path2 = "Gaul/"; else if (i / 10 == 3) path2 = "Nature/"; else
                        if (i / 10 == 4) path2 = "Natar/"; //if (path2 == "Natar/") extension = ".png";
                        grid_Army.Rows.Add(Image.FromFile(path + path2 + (i + 1) + extension), 
                            GAME.Map.Cell[cx, cy].AllTroops[i], " " + GAME.Troops[i / 10].Information[i % 10].Name + " ");
                        if (GAME.ColorArmyFlag) {
                            var cell1 = grid_Army.Rows[grid_Army.Rows.Count - 1].Cells[1];
                            if (path2 == "Rome/") cell1.Style.ForeColor = Color.Maroon;//войска value
                            else if (path2 == "German/") cell1.Style.ForeColor = Color.Navy;//войска value
                            else if (path2 == "Gaul/") cell1.Style.ForeColor = Color.DarkGreen;//войска value
                            else if (path2 == "Natar/") cell1.Style.ForeColor = Color.Black;//войска value
                            else if (path2 == "Nature/") cell1.Style.ForeColor = Color.Purple;//войска value
                        }
                    }
                }
                //подгон ширины Grid (убираем горизонтальную полосу прокрутки)
                grid_Army.Width = grid_Army.RowHeadersWidth; for (int i = 0; i < grid_Army.Columns.Count; i++) grid_Army.Width += grid_Army.Columns[i].Width;
                //динамическое изменение высоты панели в зависимости от её свойства Top
                int gridHeight = grid_Army.ColumnHeadersHeight + grid_Army.Rows.Count * grid_Army.Rows[0].Height + ToCSR(10);
                if (gridHeight <= grid_Army.Parent.Height - grid_Army.Top - ToCSR(40)) grid_Army.Height = gridHeight;
                else grid_Army.Height = grid_Army.Parent.Height - grid_Army.Top - ToCSR(40);
                
                //выставляем позицию скролла в запоминаемую позицию
                try { grid_Army.FirstDisplayedScrollingRowIndex = FDSRI; } catch { FDSRI = 0; }
            }
            //выравниваем эту панель под ширину панели добычи ресурсов
            if ((grid_Army.Width < Panel_Resources_Production.Width) ||
                (grid_Army.Width > Panel_Resources_Production.Width && TroopCount <= 0))
                    grid_Army.Width = Panel_Resources_Production.Width;
            Panel_Army.Width = grid_Army.Width;
            grid_Army.Location = new Point(Panel_Army.Left, Panel_Army.Bottom);//прикрепляем таблицу к панели
            //grid_Army.ClearSelection();
            grid_Army.Visible = true;//показываем таблицу
        }

        public void Update_Picture_Ethnos() {
            string[] path = new string[] { "DATA_BASE/IMG/natiions/Rome1.png", "DATA_BASE/IMG/natiions/Rome2.png",
                "DATA_BASE/IMG/natiions/Teutons1.png", "DATA_BASE/IMG/natiions/Teutons2.png",
                "DATA_BASE/IMG/natiions/Gauls1.png", "DATA_BASE/IMG/natiions/Gauls2.png",
            }; int N = 21, min = 0, max = 1;
            switch (Player.Folk_Name) { 
                case Folk.Rome: N = 21; min = 0; max = 1; break;
                case Folk.German: N = 22; min = 2; max = 3; break;
                case Folk.Gaul: N = 23; min = 4; max = 5; break;
                default: MessageBox.Show(LANGUAGES.Errors[1]/*Error 1.*/ + " form1.Update_Picture_Ethnos(); Player.Folk_Name = '" + Player.Folk_Name + "',"
                    + "\n" + LANGUAGES.Errors[21]);/*вместо того чтобы быть равно: [Римляне = 0, Германцы = 1, Галлы = 2]. Картинка этноса не загружена.*/
                    break;
            }
            lb_Ethnos.Text = LANGUAGES.RESOURSES[N].ToUpper();//Римляне Германцы Галлы
            Picture_Ethnos.BackgroundImage = Image.FromFile(path[Random.RND(min, max)]);
        }

        public void Update_Buttons_Level_Slots(Slots slots) {
            //ресурсные поля
            Color cl; FontStyle fs;
            if (slots == Slots.Resources) for (int i = 0; i < Button_Slot_Resource.Length; i++) {
                Button_Slot_Resource[i].Text = Player.VillageList[Player.ActiveIndex].Slot[i].Level.ToString();
                if (Player.VillageList[Player.ActiveIndex].Slot[i].ProcessOfConstruction) { cl = Color.Red; fs = FontStyle.Bold; }
                else if (Button_Slot_Resource[i].Text == "0") { cl = Color.Silver; fs = FontStyle.Regular; } 
                else { cl = Color.Black; fs = FontStyle.Bold; }
                Button_Slot_Resource[i].ForeColor = cl; Button_Slot_Resource[i].SizeFont(Button_Slot_Resource[i].Font.Size, fs);
            }
            //постройки в деревне
            //int z = 0;
            else if (slots == Slots.Village) for (int i = 0; i < Button_Slot_Builds.Length; i++) {
                int N = i + 18;
                Button_Slot_Builds[i].Text = Player.VillageList[Player.ActiveIndex].Slot[N].Level.ToString();
                if (Player.VillageList[Player.ActiveIndex].Slot[N].ProcessOfConstruction) { cl = Color.Red; fs = FontStyle.Bold; }
                else if (Button_Slot_Builds[i].Text == "0") { cl = Color.Silver; fs = FontStyle.Regular; }
                else { cl = Color.Black; fs = FontStyle.Bold; }
                Button_Slot_Builds[i].ForeColor = cl; Button_Slot_Builds[i].SizeFont(Button_Slot_Builds[i].Font.Size, fs);

                bool Слот_Строится = Player.VillageList[Player.ActiveIndex].Slot[N].ProcessOfConstruction;
                int lvl = Player.VillageList[Player.ActiveIndex].Slot[N].Level;
                Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[N].ID;
                Buildings ID_Строящегося = Player.VillageList[Player.ActiveIndex].Slot[N].ID_ProcessOfConstruction;
                if (ID == Buildings.ПУСТО && Слот_Строится) { ID = ID_Строящегося; }
                        //ТЕСТ НАТАРСКИХ СТЕН
                        //if ((int)ID >= 31 && (int)ID <= 34) ID = Buildings.Стена_Натары;
                string path = "DATA_BASE/IMG/building/"; 
                if (lvl == 0) { switch (ID) {
                    //пустые слоты
                    case Buildings.Пункт_сбора:
                        string s1; if (Слот_Строится) s1 = path + "g" + (int)ID + "b.png"; else s1 = path + "slot_punkt_sbora.png";
                        img_Slot_builds[i].BackgroundImage = Image.FromFile(s1);
                        /*y1080*/if (SCREEN(max:1080)) { img_Slot_builds[i].Location(0.56 - 0.014,      0.3520 - 0.0012); }//+
                        /*y900*/else if (SCREEN(900, 1080)) { img_Slot_builds[i].Location(0.56 - 0.014, 0.3365 - 0.0012); }//+
                        /*y768*/else if (SCREEN(768, 900)) { img_Slot_builds[i].Location(0.56 - 0.014,  0.3232 - 0.0012); }//+
                        /*y720*/else if (SCREEN(min: 768)) { img_Slot_builds[i].Location(0.56 - 0.014,  0.3100 - 0.0012); }//+
                        Button_Slot_Builds[i].Centering(); Button_Slot_Builds[i].Left += ToCSR(30);
                    break;
                    case Buildings.Городская_стена_Римляне: case Buildings.Земляной_вал_Германцы:
                    case Buildings.Изгородь_Галлы:          case Buildings.Стена_Натары:
                                img_Slot_builds[i].BackgroundImage = null;
                    break;
                    default:
                        Button_Slot_Builds[i].Location = Button_Slot_Location[i];
                        //Слот_Строится = true; ID = 23 + z; z++; if (ID > 40 || (ID>=31 && ID<=33)) ID = 40;//для теста
                        //код отображения всех остальных построек в процессе строительства
                        string s2; if (Слот_Строится) { string ext; 
                            if (ID != Buildings.Госпиталь) { ext = "gif"; img_Slot_builds[i].Size(75 + 10, 100); } 
                            else { ext = "png"; img_Slot_builds[i].Size(100, 100); }//исключение для госпиталя [12]
                            s2 = path + "g" + (int)ID + "b." + ext;
                        } else { s2 = path + "slot_build.png"; img_Slot_builds[i].Size(100, 100); }
                        img_Slot_builds[i].BackgroundImage = Image.FromFile(s2);
                    break;
                }} else { switch (ID) {
                    //слоты с постройками
                    case Buildings.Главное_здание:
                        img_Slot_builds[i].Size(140, 140);
                        img_Slot_builds[i].BackgroundImage = Image.FromFile($"{path}g{(int)ID}-ltr.png");
                        /*y1080*/if (SCREEN(max:1080)) { img_Slot_builds[i].Location(0.483 - 0.06,      0.2110 - 0.00); }//+
                        /*y900*/else if (SCREEN(900, 1080)) { img_Slot_builds[i].Location(0.483 - 0.06, 0.2050 - 0.00); }//+
                        /*y768*/else if (SCREEN(768, 900)) { img_Slot_builds[i].Location(0.483 - 0.06,  0.1925 - 0.00); }//+
                        /*y720*/else if (SCREEN(min: 768)) { img_Slot_builds[i].Location(0.483 - 0.06,  0.1800 - 0.00); }//+
                        Button_Slot_Builds[i].Centering();
                    break;
                    case Buildings.Пункт_сбора:
                        img_Slot_builds[i].BackgroundImage = Image.FromFile($"{path}g{(int)ID}-ltr2.png");
                        /*y1080*/if (SCREEN(max:1080)) { img_Slot_builds[i].Location(0.57 - 0.016,      0.3380 + 0.008); }//+
                        /*y900*/else if (SCREEN(900, 1080)) { img_Slot_builds[i].Location(0.57 - 0.016, 0.3200 + 0.008); }//+
                        /*y768*/else if (SCREEN(768, 900)) { img_Slot_builds[i].Location(0.57 - 0.016,  0.3085 + 0.008); }//+
                        /*y720*/else if (SCREEN(min: 768)) { img_Slot_builds[i].Location(0.57 - 0.016,  0.2940 + 0.008); }//+
                        Button_Slot_Builds[i].Centering();
                    break;
                    case Buildings.Городская_стена_Римляне: case Buildings.Земляной_вал_Германцы:
                    case Buildings.Изгородь_Галлы:          case Buildings.Стена_Натары: {
                        int n = 3; if (lvl >= 1 && lvl <= 4) n = 3; else if (lvl >= 5 && lvl <= 9) n = 4;
                        else if (lvl >= 10 && lvl <= 14) n = 5; else if (lvl >= 15 && lvl <= 20) n = 6;
                        try { img_Slot_builds[i].BackgroundImage = Image.FromFile($"{path}g{(int)ID}-ltr{n}.png"); }
                        catch { img_Slot_builds[i].BackgroundImage = Image.FromFile($"{path}g{(int)ID}-ltr3.png"); }
                    } break;
                    default:
                        img_Slot_builds[i].Size(100, 100);
                        img_Slot_builds[i].BackgroundImage = Image.FromFile($"{path}g{(int)ID}-ltr.png");
                        Button_Slot_Builds[i].Centering();
                    break;
                }}
                //img_Slot_builds[i].Contour_Solid(2, Color.Black);//рамка для теста разметки элементов
                img_Slot_builds[i].BringToFront(); Button_Slot_Builds[i].BringToFront();
                if (ID >= Buildings.Городская_стена_Римляне && ID <= Buildings.Стена_Натары) { img_Slot_builds[i].SendToBack(); bg_Village.SendToBack(); }
            }
        }
        //======================================================= ИНТЕРФЕЙС ======================================================
    }
}
