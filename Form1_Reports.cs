using GameLogica;
using System;
using System.Drawing;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using static UFO.Convert;

namespace WindowsInterface {
    public partial class Form1 : Form {
        /// <summary> Таблица grid_Reports. Цвет текста 3 столбца "События", ячейки с не прочитанным отчётом. </summary>
        private Color grid_Reports_Cell_3_Color_Read_false = Color.FromArgb(135, 180, 30);
        /// <summary> Таблица grid_Reports. Цвет текста 3 столбца "События", ячейки с прочитанным отчётом. </summary>
        private Color grid_Reports_Cell_3_Color_Read_true = Color.FromArgb(135, 180, 30);

        /// <summary> Метод обрабатывает клик по ячейкам таблицы <b>grid_Reports.</b> </summary>
        private void grid_Reports_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 3) if (e.RowIndex >= 0) {
                //открыть окно отчётов
                WinDlg_Multi_Reports(e.RowIndex, (TReport.TData)((DataGridView)sender).Rows[e.RowIndex].Cells[0].Tag);
                Reports_ReadNotRead(true, e.RowIndex);//помечаем выбранный отчёт как прочитанный
            }
            var _ = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.ColumnIndex == 0) _.Value = !(bool)_.Value;//чекаем-не чекаем боксы
            ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;//отмена выделения ячейки
        }
        /// <summary> Метод обрабатывает наведение на ячейку <b>grid_Reports.</b> </summary>
        private void grid_Reports_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 3) if (e.RowIndex >= 0) {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки <b>grid_Reports.</b> </summary>
        private void grid_Reports_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 3) if (e.RowIndex >= 0) {
                if (((TReport.TData)((DataGridView)sender).Rows[e.RowIndex].Cells[0].Tag).Read)
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = grid_Reports_Cell_3_Color_Read_true;
                else ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = grid_Reports_Cell_3_Color_Read_false;
            }
        }

        /// <summary> Проверка переданного отчёта кнопоками-фильтрами на предмет можно ли его добавлять в таблицу. </summary>
        /// <value>
        ///     <b> <paramref name="i"/>: </b> номер отчёта в листе <b>GAME.Report.LIST[i]</b> <br/>
        ///     <b> <paramref name="btn"/>: </b> ключевая кнопка-фильтр которую следует проверить. <br/>
        /// </value>
        /// <returns> Возвращает <b>true</b> если отчёт можно вывести в таблицу и <b>false</b> - если отчёт не проходит фильтры. </returns>
        private bool Filter(int i, Button btn) {
            //проверяем все случаи когда отчёт выводить нельзя
            var Cell_PlayerActiveVillage = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);
            var Cell_Start = GAME.Reports.LIST[i].Cell_Start;
            var Cell_Finish = GAME.Reports.LIST[i].Cell_Finish;
            if (GAME.Reports.LIST[i].Archive) { if ((int)btn_Reports_Archive.Tag == 0) return false; } else {
                if ((int)btn_Reports_All.Tag == 0 && (int)btn.Tag == 0) if ((int)btn_Reports_Neighborhood.Tag == 1) {
                    if (Cell_PlayerActiveVillage.X - Cell_Start.X + Cell_PlayerActiveVillage.Y - Cell_Start.Y == 0) {
                        if (Math.Abs(Cell_PlayerActiveVillage.X - Cell_Finish.X) > GAME.Radius_Neighborhood ||
                            Math.Abs(Cell_PlayerActiveVillage.Y - Cell_Finish.Y) > GAME.Radius_Neighborhood) return false;
                    } else if (Math.Abs(Cell_PlayerActiveVillage.X - Cell_Start.X) > GAME.Radius_Neighborhood ||
                                Math.Abs(Cell_PlayerActiveVillage.Y - Cell_Start.Y) > GAME.Radius_Neighborhood) return false;
                    } else return false; }
            return true;
        }

        /// <summary> Метод вычисляет загружаемую картинку "фарм" с помощью суммы грузоподъёмности юнитов и суммы добытых ресурсов в отчёте <b>GAME.Reports.LIST[i];</b> <br/> Метод возвращает множественные значения. </summary>
        /// <value>
        ///     <b> <paramref name="i"/>: </b> номер отчёта в листе <b>GAME.Report.LIST[i]</b> <br/>
        ///     <b> <paramref name="ToolTipText2"/>: </b> вторая всплывающая подсказка для таблицы <b>grid_Reports.</b>. <br/>
        /// </value>
        /// <returns> 
        ///     Возвращает картинку соответствующую сумме грузоподъёмности ресурсов и сумме добытого (награбленного): <br/>
        ///     <b>farm_0.png</b> - награбленные ресурсы отсутствуют. <br/>
        ///     <b>farm_1.png</b> - есть награбленные ресурсы, но суммарная грузоподъёмность войск больше фактически награбленного. <br/>
        ///     <b>farm_2.png</b> - есть награбленные ресурсы, но суммарная грузоподъёмность войск меньше или равна фактически награбленному. <br/>
        ///     <b>null.png</b> - отсутствует нужная картинка в папке пиктограмм. <br/><br/>
        ///     Переданная переменная в метод Farm() <b>out string ToolTipText2</b> получает строку типа: "217/5600" для дальнейшего использования в таблице, <br/> где первое число = сумма фарма / второе число = грузоподъёмность выживших войск.
        /// </returns>
        private Image Farm(TReport.TData report, out string ToolTipText2) {
            var Cell_Start = report.Cell_Start;
            var Start_LinkAccount = GAME.Map.Cell[Cell_Start.X, Cell_Start.Y].LinkAccount;
            int t = (int)Start_LinkAccount.Folk_Name;//номер нации в массиве GAME.Troops[t] атакующего
            int SumLoadCapacity = 0;//сумма грузоподъёмности ресурсов всех выживших юнитов атакующего
            int SumResources = 0;//сумма всех добытых ресурсов атакующего
            for (int n = 0; n < report.Units.Length - 1; n++) SumLoadCapacity += report.Units[n] * (int)GAME.Troops[t].Information[n].LoadCapacity;
            for (int n = 0; n < report.Resources.Length; n++) SumResources += report.Resources[n];
            ToolTipText2 = toTABString(SumResources.ToString(), "'") + "/" + toTABString(SumLoadCapacity.ToString(), "'");
            if (SumResources <= 0) return Image.FromFile("DATA_BASE/IMG/pictograms/ico/farm_0.png");
            else if (SumResources < SumLoadCapacity) return Image.FromFile("DATA_BASE/IMG/pictograms/ico/farm_1.png");
            else if (SumResources >= SumLoadCapacity) return Image.FromFile("DATA_BASE/IMG/pictograms/ico/farm_2.png");
            else return Image.FromFile("DATA_BASE/IMG/pictograms/ico/null.png");
        }

        /// <summary> Метод помечает строку в таблице отчётов как прочитанный/не прочитанный отчёт. </summary>
        /// <value>
        ///     <b> <paramref name="Read"/>: </b> флаг отчёта, <b>true</b> = отчёт прочитан, <b>false</b> = отчёт не прочитан. <br/>
        ///     <b> <paramref name="Line"/>: </b> номер строки таблицы <b>grid_Reports</b>, в которой произойдут изменения. <br/>
        /// </value>
        public void Reports_ReadNotRead(bool Read, int Line) {
            FontStyle Font_Style; float FontSize; Color cl;
            if (Read) { cl = grid_Reports_Cell_3_Color_Read_false;
                Font_Style = FontStyle.Regular; FontSize = ToCSR(10);
                grid_Reports.Rows[Line].Cells[1].ToolTipText = LANGUAGES.Reports[39];/*Отчёт прочитан.*/  //всплывающая подсказка
            } else { cl = grid_Reports_Cell_3_Color_Read_true;
                Font_Style = FontStyle.Bold; FontSize = ToCSR(11);
                grid_Reports.Rows[Line].Cells[1].ToolTipText = LANGUAGES.Reports[40];/*Отчёт не прочитан.*/  //всплывающая подсказка
            }
            grid_Reports.Rows[Line].Cells[3].Style.Font =
                new Font(grid_Reports.Columns[2].DefaultCellStyle.Font.FontFamily, FontSize, Font_Style);
            grid_Reports.Rows[Line].Cells[3].Style.ForeColor = cl;
        }

        /// <summary> Метод обновляет вкладку "Отчёты". </summary>
        public void Update_Panels_Reports() {
            NewPositionButtons_pnl_Reports_Buttons();//положение кнопок на панели pnl_Reports_Buttons
            //лепим панели по центру
            pnl_Reports_Head.Centering_X(ToCSR(140));
            pnl_Reports_Buttons.Centering_X(pnl_Reports_Head.Top + pnl_Reports_Head.Height);
            grid_Reports.Centering_X(pnl_Reports_Buttons.Top + pnl_Reports_Buttons.Height + ToCSR(10));
            pnl_Reports_Footer.Centering_X(pnl_Reports_Footer.Parent.Height - pnl_Reports_Footer.Height - ToCSR(10));
            grid_Reports.Height = pnl_Reports_Footer.Top - grid_Reports.Top - ToCSR(10);
            NewPositionControls_pnl_Reports_Footer();//положение контролов на панели pnl_Reports_Footer
            grid_Reports.Rows.Clear(); chb_Reports_All.Checked = false;

            //если есть хотя бы один отчёт
            if (GAME.Reports.LIST.Count > 0) {
                for (int i = GAME.Reports.LIST.Count - 1; i >= 0 ; i--) { //самый свежий отчёт появляется вверху таблицы
                    var Cell_Start = GAME.Reports.LIST[i].Cell_Start;
                    var Cell_Finish = GAME.Reports.LIST[i].Cell_Finish;
                    var Start_LinkVillage = GAME.Map.Cell[Cell_Start.X, Cell_Start.Y].LinkVillage;
                    var Finish_LinkVillage = GAME.Map.Cell[Cell_Finish.X, Cell_Finish.Y].LinkVillage;
                    var Start_LinkAccount = GAME.Map.Cell[Cell_Start.X, Cell_Start.Y].LinkAccount;
                    var Finish_LinkAccount = GAME.Map.Cell[Cell_Finish.X, Cell_Finish.Y].LinkAccount;
                    string VillageName1 = Start_LinkVillage == null ? "[?]" : Start_LinkVillage.Village_Name;
                    string VillageName2 = Finish_LinkVillage == null ? "[?]" : Finish_LinkVillage.Village_Name;
                    string AccountName1 = Start_LinkAccount == null ? "[?]" : Start_LinkAccount.Nick_Name;
                    string AccountName2 = Finish_LinkAccount == null ? "[?]" : Finish_LinkAccount.Nick_Name;
                    //если одна из координат отчёта принадлежит игроку Player или выбрана кнопка "Окрестность"
                    if ((((int)btn_Reports_Neighborhood.Tag == 0) && (AccountName1 == Player.Nick_Name
                        || AccountName2 == Player.Nick_Name)) || ((int)btn_Reports_Neighborhood.Tag == 1)) {
                        string EventHeadText = "";//текст заголовка отчёта [событие]
                        string ToolTipText1 = "";//текст всплывающей подсказки уточняющей заголовок отчёта
                        string ToolTipText2 = "";//текст всплывающей подсказки уточняющей вторую пиктограмму
                        bool Add_Line = false;//флаг добавления строки в таблицу. true = добавить строку
                        bool path_ico = true;//флаг пиктограммы. false = пиктограммы нет, выводить ico/null.png и текст всплывающей подсказки LANGUAGE.Reports[13]
                        var TR = (int)GAME.Reports.LIST[i].TypeReport;
                        /*пиктограммы*/
                        Image[] img = new Image[3];//default
                        img[0] = new Bitmap(1, 1); img[1] = new Bitmap(1, 1); img[2] = new Bitmap(1, 1);
                        switch (GAME.Reports.LIST[i].TypeReport) {
                            //ПАПКА MOVE
                            case Type_Report.SettlersCreateNewVillage:
                                if (!Filter(i, btn_Reports_All)) break;//проверка кнопок-фильтров
                                EventHeadText = Start_LinkVillage.Village_Name + " " + LANGUAGES.Reports[58]/*основал поселение*/ + " " + Finish_LinkVillage.Village_Name;
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/settlers2.png");
                                ToolTipText1 = LANGUAGES.Reports[59]/*Отчёт о новом поселении*/;
                                Add_Line = true;
                                break;
                            case Type_Report.Attack_Win_GREEN:     case Type_Report.Attack_Losses_YELLOW:
                            case Type_Report.Attack_Dead_RED:      case Type_Report.Defend_Win_GREEN:
                            case Type_Report.Defend_Losses_YELLOW: case Type_Report.Defend_Dead_RED:
                            case Type_Report.Defend_Win_GRAY:
                                if (!Filter(i, btn_Reports_Army)) break;//проверка кнопок-фильтров
                                string s; if (GAME.Reports.LIST[i].TypeEvent == Type_Event.ATTACK) s = LANGUAGES.Reports[31];/*атаковал*/
                                else s = LANGUAGES.Reports[43];/*совершил набег на*/
                                EventHeadText = VillageName1 + " " + s + " " + VillageName2;
                                string FileName = TR == 0 ? "move/GREEN_attacker1.png" : TR == 1 ? "move/YELLOW_attacker1.png" :
                                                  TR == 2 ? "move/RED_attacker1.png" : TR == 3 ? "move/won_defender1.png" :
                                                  TR == 4 ? "move/won_defender3.png" : TR == 5 ? "move/lost_defender1.png" :
                                                  TR == 6 ? "move/lost_defender.png" : "null.png";
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/" + FileName);
                                img[2] = Farm(GAME.Reports.LIST[i], out ToolTipText2);
                                ToolTipText2 = LANGUAGES.Reports[15]/*Добыча*/ + " " + ToolTipText2;
                                /*32 Как нападающий, вы выиграли без потерь.*/         /*33 Как нападающий, вы выиграли, но с потерями.*/
                                /*34 Как нападающий, вы проиграли (никто не выжил).*/  /*35 Как защитник, вы выиграли без потерь.*/
                                /*36 Как защитник, вы выиграли, но с потерями.*/       /*37 Как защитник, вы проиграли (никто не выжил).*/
                                /*38 Как защитник, вы выиграли без потерь (у вас не было войск).*/
                                ToolTipText1 = LANGUAGES.Reports[TR + 32];
                                if (FileName == "null.png") path_ico = false;//нет нужной пиктограммы
                                Add_Line = true;
                                break;
                            case Type_Report.Reinforcement_Arrived:
                                if (!Filter(i, btn_Reports_Army)) break;//проверка кнопок-фильтров
                                EventHeadText = VillageName2 + " " + LANGUAGES.Reports[12]/*получил подкрепление из*/ + " " + VillageName1;
                                string FileName2 = AccountName1 == Player.Nick_Name ? 
                                    "reinforcement_arrived.png" : "reinforcement_arrived_2.png";
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/move/" + FileName2);
                                ToolTipText1 = LANGUAGES.Reports[14]/*Подкрепление игрока*/ + " " + AccountName1;
                                Add_Line = true;
                                break;
                            //ПАПКА OTHER
                            case Type_Report.Miscellaneous:
                                break;
                            case Type_Report.Adventure:
                                if (!Filter(i, btn_Reports_Other)) break;//проверка кнопок-фильтров
                                EventHeadText = VillageName1 + " " + LANGUAGES.Reports[41]/*исследовал*/ + " (" + GAME.Map.Coord_MapToWorld(Cell_Finish).X + " | " + GAME.Map.Coord_MapToWorld(Cell_Finish).Y + ")";
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/other/" + GAME.Reports.LIST[i].TypeReport + ".png");
                                img[2] = Farm(GAME.Reports.LIST[i], out ToolTipText2);
                                ToolTipText2 = LANGUAGES.Reports[15]/*Добыча*/ + " " + ToolTipText2;
                                ToolTipText1 = LANGUAGES.Reports[42];/*Отчёт о приключении.*/
                                Add_Line = true;
                                break;
                            case Type_Report.Animals_Caught:
                                break;
                            case Type_Report.Won_Scout_Attacker_GREEN: case Type_Report.Won_Scout_Attacker_YELLOW:
                            case Type_Report.Lost_Scout_Attacker_RED:  case Type_Report.Won_Scout_Defender_GREEN: 
                            case Type_Report.Lost_Scout_Defender_YELLOW:
                                if (!Filter(i, btn_Reports_Other)) break;//проверка кнопок-фильтров
                                EventHeadText = VillageName1 + " " + LANGUAGES.Reports[23]/*разведал*/ + " " + VillageName2;
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/other/" + GAME.Reports.LIST[i].TypeReport + ".png");
                                ToolTipText1 = LANGUAGES.Reports[(int)GAME.Reports.LIST[i].TypeReport + 13];/*24-28 текст про разведку*/
                                Add_Line = true;
                                break;
                            //ПАПКА TRADING
                            case Type_Report.Mostly_wood: case Type_Report.Mostly_clay: case Type_Report.Mostly_iron:
                            case Type_Report.Mostly_crop: case Type_Report.Mostly_gold:
                                if (!Filter(i, btn_Reports_Trading)) break;//проверка кнопок-фильтров
                                EventHeadText = VillageName1 + " " + LANGUAGES.Reports[16]/*снабжает*/ + " " + VillageName2;
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/trading/" + GAME.Reports.LIST[i].TypeReport + ".png");
                                ToolTipText1 = LANGUAGES.Reports[17]/*Торговец доставил в основном*/ + 
                                               " " + LANGUAGES.Reports[(int)GAME.Reports.LIST[i].TypeReport + 2];/*wood clay iron crop gold*/
                                Add_Line = true;
                                break;
                            case Type_Report.Reinforcements_Attacked:
                                if (!Filter(i, btn_Reports_Army)) break;//проверка кнопок-фильтров
                                EventHeadText = LANGUAGES.Reports[44]/*Ваш подкреп. в*/ + " " + VillageName2 + " " + LANGUAGES.Reports[45];/*был атакован.*/
                                int sum_DVT = 0, sum_DVL = 0;
                                for (int n = 0; n < GAME.Reports.LIST[i].Attack_Troops.Length; n++) {
                                    sum_DVT += GAME.Reports.LIST[i].Attack_Troops[n];//обороняющиеся
                                    sum_DVL += GAME.Reports.LIST[i].Attack_Losses[n];//убитые
                                }
                                string FileName3 = sum_DVL <= 0 ? "move/won_defender1.png"/*GREEN*/ :
                                sum_DVL < sum_DVT ? "move/won_defender3.png"/*YELLOW*/ : "move/lost_defender1.png";/*RED*/
                                img[1] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/" + FileName3);
                                ToolTipText1 = LANGUAGES.Reports[46]/*Ваше подкрепление размещено у игрока*/ + " " + AccountName2;
                                Add_Line = true;
                                break;
                        }
                        if (Add_Line) {
                            string FN = "read_" + GAME.Reports.LIST[i].Read + ".png";
                            img[0] = Image.FromFile("DATA_BASE/IMG/pictograms/ico/" + FN);
                            //ресайз картинок с учётом разрешения экрана
                            Image[] ico = new Image[3]; 
                            ico[0] = Extensions.ResizeImage(img[0], ToCSR(19), ToCSR(16));//mail read/not read
                            ico[1] = Extensions.ResizeImage(img[1], ToCSR(23), ToCSR(25));//type report
                            ico[2] = Extensions.ResizeImage(img[2], ToCSR(19), ToCSR(20));//farm resources-count

                            //добавление строки с данными
                            string DateTime = GAME.Reports.LIST[i].Date + " " + LANGUAGES.Reports[47]/*в*/ + " " + GAME.Reports.LIST[i].Time;
                            grid_Reports.Rows.Add(false, ico[0], ico[1], EventHeadText, ico[2], DateTime);

                            int push_back = grid_Reports.Rows.Count - 1;
                            grid_Reports.Rows[push_back].Cells[0].Tag = GAME.Reports.LIST[i];//первая ячейка строки хранит ссылку отчёта в LIST
                            //добавление всплывающей подсказки на строку
                            for (int j = 2; j < 4; j++) grid_Reports.Rows[push_back].Cells[j].ToolTipText = ToolTipText1;
                            grid_Reports.Rows[push_back].Cells[4].ToolTipText = ToolTipText2;
                            if (!path_ico) grid_Reports.Rows[push_back].Cells[1].ToolTipText = LANGUAGES.Reports[13];/*[null] Пиктограмма не найдена*/
                            //дополнительно редактируем заголовки прочитаных/не прочитанных отчётов
                            Reports_ReadNotRead(GAME.Reports.LIST[i].Read, push_back);
                        }
                    }
                }
            }
            //скрыть выделение ячеек
            grid_Reports.DefaultCellStyle.SelectionBackColor = grid_Reports.DefaultCellStyle.BackColor;
            grid_Reports.DefaultCellStyle.SelectionForeColor = grid_Reports.DefaultCellStyle.ForeColor;
            grid_Reports.Focus();
        }

        /// <summary> Метод вычисляет новую позицию кнопок-фильтров на панели pnl_Reports_Buttons. </summary>
        public void NewPositionButtons_pnl_Reports_Buttons() {
            btn_Reports_All.Centering_Y(ToCSR(20));
            btn_Reports_Army.Location = new Point(btn_Reports_All.Right + ToCSR(10), btn_Reports_All.Top);
            btn_Reports_Trading.Location = new Point(btn_Reports_Army.Right + ToCSR(10), btn_Reports_All.Top);
            btn_Reports_Other.Location = new Point(btn_Reports_Trading.Right + ToCSR(10), btn_Reports_All.Top);
            btn_Reports_Archive.Location = new Point(btn_Reports_Other.Right + ToCSR(10), btn_Reports_All.Top);
            btn_Reports_Neighborhood.Location = new Point(btn_Reports_Archive.Right + ToCSR(10), btn_Reports_All.Top);
        }
        /// <summary> Метод вычисляет новую позицию контролов на панели pnl_Reports_Footer. </summary>
        private void NewPositionControls_pnl_Reports_Footer() {
            chb_Reports_All.Left = ToCSR(20);
            btn_Reports_Footer_Delete.Left = chb_Reports_All.Right + ToCSR(10);
            btn_Reports_Footer_InArchive.Left = btn_Reports_Footer_Delete.Right + ToCSR(10);
            btn_Reports_Footer_Read.Left = btn_Reports_Footer_InArchive.Right + ToCSR(10);
        }

        /// <summary> Логика чек бокса: "отметить всё". </summary>
        private void chb_Reports_All_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < grid_Reports.Rows.Count; i++) {
                grid_Reports.Rows[i].Cells[0].Value = chb_Reports_All.Checked;
            }
        }

        /// <summary> Логика безвозвратного удаления выделенных отчётов из таблицы и списка: <b>GAME.Reports.LIST[n];</b> </summary>
        private void btn_Reports_Footer_Delete_Click(object sender, EventArgs e) {
            for (int i = 0; i < grid_Reports.Rows.Count; i++) {
                if ((bool)grid_Reports.Rows[i].Cells[0].Value) {
                    GAME.Reports.LIST.Remove((TReport.TData)grid_Reports.Rows[i].Cells[0].Tag);
                }
            }
            _Update();
        }

        /// <summary> Логика добавления отчётов в архив листа <b>GAME.Reports.LIST[n];</b>. </summary>
        private void btn_Reports_Footer_InArchive_Click(object sender, EventArgs e) {
            if ((int)btn_Reports_Neighborhood.Tag == 1) return;//выбрана вкладка "окресность"
            int count = 0;
            for (int i = 0; i < grid_Reports.Rows.Count; i++) {
                if ((bool)grid_Reports.Rows[i].Cells[0].Value) {
                    if (!((TReport.TData)grid_Reports.Rows[i].Cells[0].Tag).Archive) { count++;
                        ((TReport.TData)grid_Reports.Rows[i].Cells[0].Tag).Archive = true; }
                    grid_Reports.Rows[i].Cells[0].Value = false;
                }
            }
            if (count > 0) { _Update();
                MessageBox.Show(LANGUAGES.Reports[29]/*В архив добавлено*/ + " " + count + " " + LANGUAGES.Reports[30]/*отчётов*/);
            }
        }

        /// <summary> Логика отметки выбранного отчёта как прочитанного. </summary>
        private void btn_Reports_Footer_Read_Click(object sender, EventArgs e) {
            for (int i = 0; i < grid_Reports.Rows.Count; i++) {
                if ((bool)grid_Reports.Rows[i].Cells[0].Value) {
                    ((TReport.TData)grid_Reports.Rows[i].Cells[0].Tag).Read = true;
                    grid_Reports.Rows[i].Cells[0].Value = false;
                }
            }
            _Update();
        }
    }
}
