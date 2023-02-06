using GameLogica;
using System;
using System.Drawing;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;

namespace WindowsInterface
{
    public partial class Form1 : Form {
        /// <summary> Переменная имеет 2 состояния: 1 и 2. Нужна для загрузки двух вариантов пиктограмм передвиженяи войск для анимации. </summary>
        private byte N_pic = 1;
        /// <summary> Флаг сигнализирующий о том, что в каком-то слоте деревни завершилось строительство и требуется обновление текста соответствующей кнопки. </summary>
        private bool Flag_Village_Завершение_Строительства_Сноса = false;
        /// <summary> Флаг сигнализирующий о том, что в каком-то слоте ресурсных построек завершилось строительство и требуется обновление текста соответствующей кнопки. </summary>
        private bool Flag_Resources_Завершение_Строительства = false;
        /// <summary> Флаг сигнализирующий о том, что в деревню прибыло подкрепление. </summary>
        private bool Flag_Reinforcement = false;
        /// <summary> Флаг сигнализирующий о том, что в деревне голод и умер один боец. </summary>
        private bool Flag_Hunger = false;
        /// <summary> Координаты сработанного события строительства относительно карты <b>Map.Cell[x][y];</b> </summary>
        private Point Flag_Коры_Construction;
        /// <summary> Координаты сработанного события передвижения войск относительно карты <b>Map.Cell[x][y];</b> </summary>
        private Point Flag_Коры_Movements;
        /// <summary> Координаты сработанного события голода и смерти одного бойца. </summary>
        private Point Flag_Коры_Hunger;
        /// <summary> Номер слота игрока в котором произошло событие. </summary>
        private int Flag_Slot = -1;
        /// <summary> Счётчик-кнопка количества отчётов. Появляется на картинке-кнопке "Отчёты" когда есть отчёты и информирует о количестве. </summary>
        private Button btn_Report_Counter = new Button() { 
            ForeColor = Color.Black, BackColor = Color.White, Enabled = false, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(2, 2, 2, 2),
            Font = new Font("Arial", 8, FontStyle.Bold) };
        /// <summary> Счётчик-кнопка количества сообщений. Появляется на картинке-кнопке "Сообщения" когда есть сообщения и информирует о количестве. </summary>
        private Button btn_Message_Counter = new Button() { 
            ForeColor = Color.Black, BackColor = Color.White, Enabled = false, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(2, 2, 2, 2),
            Font = new Font("Arial", 8, FontStyle.Bold) };

        /// <summary>
        ///     Циклический обработчик событий в главном потоке. Его дёргает: <b> System.Windows.Forms.Timer.Tick </b> <br/>
        ///     Метод обрабатывает все существующие события: <br/>
        ///     - Работает с визуальными контролами. <br/>
        /// </summary>
        public void CallBack_EventHandler_Frontend(object sender, EventArgs e) {
            btn_Report_Counter.Parent = pb_Button_Reports;//нацепляем уведомление о кол-ве отчётов на картинку-кнопку "Отчёты"
            btn_Message_Counter.Parent = pb_Button_Messages;//нацепляем уведомление о кол-ве сообщений на картинку-кнопку "Сообщения"
            //кол-во не прочитанных отчётов
            int Rcount = 0; for (int i = 0; i < GAME.Reports.LIST.Count; i++) if (!GAME.Reports.LIST[i].Read) Rcount++;
            if (Rcount > 0) {
                btn_Report_Counter.Visible = true; btn_Report_Counter.Text = Rcount.ToString();
                btn_Report_Counter.Location = new Point(pb_Button_Reports.Width - btn_Report_Counter.Width, 0);
            } else btn_Report_Counter.Visible = false;
            //кол-во не прочитанных входящих сообщений
            int Mcount = 0; for (int i = 0; i < GAME.Messages.LIST.Count; i++) 
                if (GAME.Messages.LIST[i].TypeMessage == Type_Message.Incoming && !GAME.Messages.LIST[i].Read &&
                    !GAME.Messages.LIST[i].Archive) Mcount++;
            if (Mcount > 0) {
                btn_Message_Counter.Visible = true; btn_Message_Counter.Text = Mcount.ToString();
                btn_Message_Counter.Location = new Point(pb_Button_Messages.Width - btn_Message_Counter.Width, 0);
            } else btn_Message_Counter.Visible = false;

            switch (tabControl.SelectedIndex) {
                //Эффект наблюдателя на вкладке 0. МЕНЮ
                case 0: break;
                //Эффект наблюдателя на вкладке 1. ресурсные поля
                case 1:
                    //обновляем информацию в переданном слоте из таймера CallBack_EventHandler_Backend(...);
                    if (Flag_Resources_Завершение_Строительства) if (GAME.Map.Coord_MapToWorld(Flag_Коры_Construction.X, Flag_Коры_Construction.Y) == Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian) { 
                        Update_Buttons_Level_Slots(Slots.Resources);
                        Update_Panel_Resources_Production();
                        Update_Panel_VillageInfo();
                        //обновляем окно с повышением уровня постройки если оно открыто для того же слота
                        //if (ActiveSlot_in_winDlg_LevelUp_Builds == Flag_Slot) 
                        //    if (form_LevelUp != null && form_LevelUp.Visible) 
                        //        winDlg_LevelUp_Builds(ActiveSlot_in_winDlg_LevelUp_Builds);

                        Flag_Resources_Завершение_Строительства = false; 
                        Flag_Village_Завершение_Строительства_Сноса = false; 
                    }

                    if (Flag_Reinforcement || Flag_Hunger) 
                        if (GAME.Map.Coord_MapToWorld(Flag_Коры_Movements.X, Flag_Коры_Movements.Y) == Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian
                            || GAME.Map.Coord_MapToWorld(Flag_Коры_Hunger.X, Flag_Коры_Hunger.Y) == Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian) {
                                Update_Panel_Army();
                                Update_Panel_Resources_Production();
                                if (Flag_Reinforcement) Flag_Reinforcement = false; if (Flag_Hunger) Flag_Hunger = false;
                        }

                    Update_Panel_Resources();
                    Update_Panel_Troop_Movements();
                    Update_Panel_Construction();
                break;
                //Эффект наблюдателя на вкладке 2. деревня
                case 2:
                    //обновляем информацию в переданном слоте из таймера CallBack_EventHandler_Backend(...);
                    if (Flag_Village_Завершение_Строительства_Сноса) if (GAME.Map.Coord_MapToWorld(Flag_Коры_Construction.X, Flag_Коры_Construction.Y) == Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian) { 
                       SortRank(TypeStatistics.Players);//сортировка игроков в списке ListBox по населению
                       Update_Buttons_Level_Slots(Slots.Village);
                       Update_Panel_Resources_Production();
                       Update_Panel_VillageInfo();
                       //Обновляем выпадающие списки свежими строками
                       if (form_LevelUp != null && form_LevelUp.Visible) {
                           if (Build_List.Visible) {
                               if (This_List == What_In_Items.Строительство) {
                                   Build_List.Items.Clear(); var _Items = Get_ComboBox_Items();
                                   if (_Items != null) Build_List.Items.AddRange(_Items);
                                   //обновляем окно с повышением уровня постройки если оно открыто для того же слота
                                   //if (ActiveSlot_in_winDlg_LevelUp_Builds == Flag_Slot) 
                                       //winDlg_LevelUp_Builds(ActiveSlot_in_winDlg_LevelUp_Builds);
                                   Build_List.SelectedIndex = Build_List.Items.Count > BL_SelectedIndex_Save ? BL_SelectedIndex_Save : -1;
                               } else if (This_List == What_In_Items.Снос) {
                                   Build_List.Items.Clear(); var _Items = Get_ComboBox_Items_Destruction();
                                   if (_Items != null) Build_List.Items.AddRange(_Items);
                                   winDlg_LevelUp_Builds(27);//Главное Здание
                               }
                           }
                       }

                       Flag_Village_Завершение_Строительства_Сноса = false; 
                       Flag_Resources_Завершение_Строительства = false; 
                    }

                    Update_Panel_Resources();
                    Update_Panel_Construction();
                break;
                //Эффект наблюдателя на вкладке 3. карта
                case 3: 
                    Update_Panel_Resources();
                    break;
                //Эффект наблюдателя на вкладке 4. статистика
                case 4:
                    SortRank(TypeStatistics.Players);//сортировка игроков в списке ListBox по населению
                    Update_Panel_Resources();
                    break;
                //Эффект наблюдателя на вкладке 5. отчёты
                case 5:
                    Update_Panel_Resources();
                    break;
                //Эффект наблюдателя на вкладке 6. сообщения
                case 6:
                    Update_Panel_Resources();
                    break;
                default: break;
            }
        }

        /// <summary>
        ///     Циклический обработчик событий в отдельном фоновом потоке. Его дёргает: <b> System.threading.Timer </b> <br/>
        ///     Метод обрабатывает все существующие события: <br/>
        ///     - Вычитает время из таймеров на стеке событий. <br/>
        ///     - Удаляет строки в стеке по мере необходимости. <br/>
        ///     - Добавляет ресурсы в хранилища из выработки в час каждому аккаунту. <br/>
        ///     - Осуществляет рост культуры каждого аккаунта. <br/>
        ///     - Осуществляет обучение юнитов в казарме, конюшне, мастерской, резиденции, дворце. <br/>
        ///     - Осуществляет отсчёт таймера праздников и торжества. <br/>
        ///     - Запускает триггеры по истечению времени таймеров (завершение постройки/обучения, генерация отчёта и т.д.). <br/>
        /// </summary>
        /// <remarks> <inheritdoc cref = "TEvent_Stack"> </inheritdoc> </remarks>
        public void CallBack_EventHandler_Backend(object obj) {
            if (obj is TGame Game && obj != null) {
                N_pic++; if (N_pic >= 3) N_pic = 1;//анимация пиктограмм на панели передвижения войск
                //СОБЫТИЯ
                for (int i = 0; i < Game.Event_Stack.Stack.Count; i++) {
                    Game.Event_Stack.Stack[i].timer--;
                    //Время вышло. Запускаем триггер и удаляем строку из списка событий.
                    int Finish_X = Game.Event_Stack.Stack[i].Cell_Finish.X;
                    int Finish_Y = Game.Event_Stack.Stack[i].Cell_Finish.Y;
                    int Start_X = Game.Event_Stack.Stack[i].Cell_Start.X;
                    int Start_Y = Game.Event_Stack.Stack[i].Cell_Start.Y;
                    var Account_Start = Game.Map.Cell[Start_X, Start_Y].LinkAccount;
                    var Village_Start = Game.Map.Cell[Start_X, Start_Y].LinkVillage;
                    var Account_Finish = Game.Map.Cell[Finish_X, Finish_Y].LinkAccount;
                    var Village_Finish = Game.Map.Cell[Finish_X, Finish_Y].LinkVillage;
                    if (Game.Event_Stack.Stack[i].timer < 0) {
                        bool DeleteLineInList = false;//true = удалить строку в листе стека, false = оставить строку в покое
                        switch (Game.Event_Stack.Stack[i].TypeEvent) {
                            case Type_Event.ATTACK:
                                //проверка существования деревни >> симуляция боя >> фарм ресурсов выжившими войсками >>
                                //формирование отчёта >> изменение статуса Type_Event.ATTACK на Type_Event.REINFORCEMENTS >>
                                //увеличение времени до первоначального если выжил хотя бы один воин >>
                                //удаление строки если никто не выжил

                                //если в событии есть хотя бы один воин, разворачиваем войска обратно владельцу
                                //сделать проверку на остаток войск после битвы, а не то что сейчас !!!!!!!!!!!!!
                                if (Game.Event_Stack.Stack[i].IsTroops()) {
                                    Game.Event_Stack.Stack[i].ReverseMoveTroops(Game.Troops[(int)Game.Event_Stack.Stack[i].Nation].Information, Account_Start.Hero, Game.Map);
                                    break;//выход из switch
                                }

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                break;
                            case Type_Event.RAID:
                                //проверка существования деревни >> симуляция боя >> фарм ресурсов выжившими войсками >>
                                //формирование отчёта >> изменение статуса Type_Event.ATTACK на Type_Event.REINFORCEMENTS >>
                                //увеличение времени до первоначального если выжил хотя бы один воин >>
                                //удаление строки если никто не выжил

                                //если в событии есть хотя бы один воин, разворачиваем войска обратно владельцу
                                //сделать проверку на остаток войск после битвы, а не то что сейчас !!!!!!!!!!!!!
                                if (Game.Event_Stack.Stack[i].IsTroops()) {
                                    Game.Event_Stack.Stack[i].ReverseMoveTroops(Game.Troops[(int)Game.Event_Stack.Stack[i].Nation].Information, Account_Start.Hero, Game.Map);
                                    break;//выход из switch
                                }

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                break;
                            //##################################################### ПОДКРЕП ###################################################
                            case Type_Event.REINFORCEMENTS:
                                //ОБРАБАТЫВАЕМ ПРИБЫВШИЕ-ОТПРАВЛЕННЫЕ ВОЙСКА В КАЧЕСТВЕ ПОДКРЕПА
                                //а как быть с оазисом? там тоже null
                                //если во время марша подкрепа обе деревни отсутствуют, удаляем событие
                                if (Village_Finish == null && Village_Start == null) {
                                    DeleteLineInList = true;//разрешить удалить строку в листе стека
                                    break;//выход из switch
                                }
                                //если во время марша подкрепа дереви нет, разворачиваем войска обратно владельцу
                                else if (Village_Finish == null) {
                                    Game.Event_Stack.Stack[i].ReverseMoveTroops(Game.Troops[(int)Game.Event_Stack.Stack[i].Nation].Information, Account_Start.Hero, Game.Map);
                                    break;//выход из switch
                                }

                                var TypeMovement = Game.Event_Stack.Stack[i].TypeMovement;
                                var Units = Game.Event_Stack.Stack[i].Units;
                                var Nation = Game.Event_Stack.Stack[i].Nation;
                                //m = shift. сдвиг в массиве ValueAllTroops[51] в сегмент где хранятся войска нации описанные в стеке
                                int m = (int)Nation * Game.Troops[0].Information.Length;/*Lenght = 10 шт без героя*/
                                if (TypeMovement == Type_Movement.Возвращение_войск) {
                                    for (int j = 0; j < Units.Length - 1; j++) { //войска
                                        Game.Map.Cell[Finish_X, Finish_Y].VillageTroops[j] += Units[j];
                                        Game.Map.Cell[Finish_X, Finish_Y].AllTroops[j + m] += Units[j];
                                        Village_Finish.Crop_Consumption +=
                                            ((int)Game.Troops[(j + m) / 10].Information[(j + m) % 10].Consumption * Units[j]);
                                    }
                                    if (Units[10] > 0) { //герой
                                        Village_Finish.Crop_Consumption += Account_Finish.Hero.Consumption;
                                        Account_Finish.Hero.isHome = true;
                                    }
                                    //добавление нафармленных ресурсов на склад деревни если они есть
                                    var Farm = Game.Event_Stack.Stack[i].Farm;
                                    var RES = Game.Map.Cell[Finish_X, Finish_Y].LinkVillage.ResourcesInStorages;//ресурсы на складе принимающего подкреп
                                    RES.wood += Farm[0]; RES.clay += Farm[1]; RES.iron += Farm[2]; RES.crop += Farm[3]; RES.gold += Farm[4];
                                    Game.Map.Cell[Finish_X, Finish_Y].LinkVillage.ResourcesInStorages = new Res(RES.wood, RES.clay, RES.iron, RES.crop, RES.gold);
                                }
                                else if (TypeMovement == Type_Movement.Подкрепление) {
                                    for (int j = 0; j < Units.Length - 1; j++) { //войска
                                        //прописываем прибытие подкрепа финишу
                                        Game.Map.Cell[Finish_X, Finish_Y].AllTroops[j + m] += Units[j];
                                        Village_Finish.Crop_Consumption +=
                                            (int)Game.Troops[(j + m) / 10].Information[(j + m) % 10].Consumption * Units[j];
                                        //прописываем отправку подкрепа старту
                                        Game.Map.Cell[Start_X, Start_Y].AllTroops[j + m] -= Units[j];
                                        Game.Map.Cell[Start_X, Start_Y].VillageTroops[j] -= Units[j];
                                        Village_Start.Crop_Consumption -=
                                            ((int)Game.Troops[(j + m) / 10].Information[(j + m) % 10].Consumption * Units[j]);
                                    }
                                    if (Units[10] > 0) { //герой
                                        //прописываем прибытие и отправку героя
                                        Game.Map.Cell[Finish_X, Finish_Y].AllTroops[50] += Units[10];
                                        Village_Finish.Crop_Consumption += Account_Start.Hero.Consumption;
                                        Village_Start.Crop_Consumption -= Account_Start.Hero.Consumption;
                                        Account_Start.Hero.isHome = false;
                                    }
                                    //создаём запись в листах для пункта сбора
                                    string key = Village_Finish.CreateKey_Hash(Village_Finish.Reinforcements, Village_Start.Reinforcements);
                                    Village_Finish.Add_Reinforcement(key, Подкреп.Вход, Nation, new Point(Start_X, Start_Y), Units);//прописываем у принявшей стороны
                                    Village_Finish.Reinforcements[Village_Finish.Reinforcements.Count - 1].LinkAccount = Account_Start;
                                    Village_Finish.Reinforcements[Village_Finish.Reinforcements.Count - 1].LinkVillage = Village_Start;

                                    Village_Start.Add_Reinforcement(key, Подкреп.Вход, Nation, new Point(Finish_X, Finish_Y), Units);//прописываем у принявшей стороны
                                    Village_Start.Reinforcements[Village_Start.Reinforcements.Count - 1].LinkAccount = Account_Finish;
                                    Village_Start.Reinforcements[Village_Start.Reinforcements.Count - 1].LinkVillage = Village_Finish;
                                }

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                Flag_Reinforcement = true;
                                Flag_Коры_Movements = Game.Event_Stack.Stack[i].Cell_Finish;
                                break;
                            //##################################################### ПОДКРЕП ###################################################
                            case Type_Event.ADVENTURE_ATTACK:

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                break;
                            case Type_Event.ADVENTURE_RAID:

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                break;
                            case Type_Event.ESTABLISH_A_SETTLEMENT:

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                break;
                            //################################################ Construction ###################################################
                            case Type_Event.Construction: //ЗАВЕРШАЕМ СТРОИТЕЛЬСТВО ПОСТРОЙКИ
                                var Account = Game.Map.Cell[Game.Event_Stack.Stack[i].Cell_Start.X, Game.Event_Stack.Stack[i].Cell_Start.Y].LinkAccount;
                                var Village = Game.Map.Cell[Game.Event_Stack.Stack[i].Cell_Start.X, Game.Event_Stack.Stack[i].Cell_Start.Y].LinkVillage;
                                var this_Cell = Game.Map.Cell[Game.Event_Stack.Stack[i].Cell_Start.X, Game.Event_Stack.Stack[i].Cell_Start.Y];
                                var Event_ID = Game.Event_Stack.Stack[i].ID;
                                var Event_lvl = Game.Event_Stack.Stack[i].lvl;
                                var Event_Slot = Game.Event_Stack.Stack[i].Slot;
                                var Build_Information = Game.Build[(int)Event_ID].Information[Event_lvl];
                                if (Account == null || Village == null) break;
                                //МАНИПУЛЯЦИИ С ДЕРЕВНЕЙ - ЗАПОЛНЕНИЕ ВСЕХ СУЩЕСТВУЮЩИХ ПОЛЕЙ
                                Village.Slot[Event_Slot].Level = Event_lvl;
                                Village.Slot[Event_Slot].ID = Event_ID;
                                //ставим false у "ProcessOfConstruction" только если это последняя постройка в слоте
                                Point p = Game.Map.Coord_WorldToMap(Village.Coordinates_World_Travian);
                                Village.Slot[Event_Slot].ProcessOfConstruction = Game.Event_Stack.IsMoreThanOneConstructionInTheSlot(p, Event_Slot);
                                
                                if (Event_lvl <= 0 && Event_Slot > 17) if (!Village.Slot[Event_Slot].ProcessOfConstruction)
                                    if (Event_Slot != 18 && Event_Slot != 19 && Event_Slot != 27) {
                                        Village.Slot[Event_Slot].ID = Buildings.ПУСТО;
                                        Village.Slot[Event_Slot].ID_ProcessOfConstruction = Buildings.ПУСТО;
                                } else Village.Slot[Event_Slot].ID_ProcessOfConstruction = Buildings.ПУСТО;

                                int tmp_Event_lvl = Event_lvl;//если снос, берём эту переменную
                                if (Game.Event_Stack.Stack[i].Destruction) { //если это снос
                                    tmp_Event_lvl = Event_lvl + 1;
                                    Build_Information = Game.Build[(int)Event_ID].Information[tmp_Event_lvl];
                                    Village.Add_Culture_EK -= Build_Information.ProductionCulture;
                                    Village.Population -= Build_Information.Consumption;
                                    Village.Crop_Consumption -= Build_Information.Consumption;
                                } else { //если это строительство
                                    Village.Add_Culture_EK += Build_Information.ProductionCulture;
                                    Village.Population += Build_Information.Consumption;
                                    Village.Crop_Consumption += Build_Information.Consumption;
                                }
                                //МАНИПУЛЯЦИИ С КАРТОЙ
                                this_Cell.ID = Game.Map.Refresh_ID(Village.Population);//обновляем картинку населения на карте
                                //вычисляем дельту уникальных параметров 1..2 если они есть.
                                //в различные поля деревни прибавляется именно дельта, а не величина из файлов g[N]-ltr.txt
                                double delta_1 = 0; if (Build_Information.Unique_Parameter_1 > -1)
                                    delta_1 = (uint)Build_Information.Unique_Parameter_1 - Game.Build[(int)Event_ID].Information[tmp_Event_lvl - 1].Unique_Parameter_1;
                                double delta_2 = 0; if (Build_Information.Unique_Parameter_2 > -1)
                                    delta_2 = (uint)Build_Information.Unique_Parameter_2 - Game.Build[(int)Event_ID].Information[tmp_Event_lvl - 1].Unique_Parameter_2;
                                //если это снос
                                if (Game.Event_Stack.Stack[i].Destruction) { delta_1 *= -1; delta_2 *= -1; }//инверсия
                                //присваиваем уникальные параметры построек в свойства полей Игрока и его деревень
                                switch (Event_ID) {
                                    case Buildings.ЧУДО_СВЕТА: break;
                                    case Buildings.Лесопилка: Village.HourlyProductionResources.wood += delta_1; break;
                                    case Buildings.Глиняный_карьер: Village.HourlyProductionResources.clay += delta_1; break;
                                    case Buildings.Железный_рудник: Village.HourlyProductionResources.iron += delta_1; break;
                                    case Buildings.Ферма: Village.HourlyProductionResources.crop += delta_1; break;
                                    case Buildings.Лесопильный_завод: Village.HourlyProductionResources_PercentOfIncrease.wood += delta_1; break;
                                    case Buildings.Кирпичный_завод: Village.HourlyProductionResources_PercentOfIncrease.clay += delta_1; break;
                                    case Buildings.Чугунолитейный_завод: Village.HourlyProductionResources_PercentOfIncrease.iron += delta_1; break;
                                    case Buildings.Мукомольная_мельница: Village.HourlyProductionResources_PercentOfIncrease.crop += delta_1; break;
                                    case Buildings.Пекарня: Village.HourlyProductionResources_PercentOfIncrease.crop += delta_1; break;
                                    case Buildings.Склад: 
                                        if (Village.Capacity.Warehouse <= Game.Capacity_Basic.Warehouse)//это дефолтная вместимость
                                            Village.Capacity.Warehouse = 0;
                                        Village.Capacity.Warehouse += (int)delta_1;
                                        if (Village.Capacity.Warehouse <= 0) Village.Capacity.Warehouse = Game.Capacity_Basic.Warehouse;
                                    break;
                                    case Buildings.Амбар:
                                        if (Village.Capacity.Barn <= Game.Capacity_Basic.Barn)//это дефолтная вместимость
                                            Village.Capacity.Barn = 0;
                                        Village.Capacity.Barn += (int)delta_1;
                                        if (Village.Capacity.Barn <= 0) Village.Capacity.Barn = Game.Capacity_Basic.Barn;
                                        break;
                                    case Buildings.Госпиталь: /*ЭТА ПОСТРОЙКА МОЖЕТ БЫТЬ ТОЛЬКО ОДНА В ДЕРЕВНЕ*/
                                        Village.Wounded_TreatmentCostMultiplier += delta_1; Village.Wounded_DeathOfUnitsPerDay_Percent += delta_2;
                                    break;
                                    case Buildings.Кузница://нет уникальных параметров. для бонусов достаточно наличия постройки
                                    break;
                                    case Buildings.Арена: Village.BonusOfSpeed_Arena += (int)delta_1; break;
                                    case Buildings.Главное_здание: Village.BonusOfTime_Construction += (int)delta_1; break;
                                    case Buildings.Пункт_сбора: Village.QuantityOfVisibleMovements_CollectionPoint += (int)delta_1; break;
                                    case Buildings.Рынок: Village.QuantityMerchants_Available += (int)delta_1; break;
                                    case Buildings.Посольство: Village.Capacity.Embassy += (int)delta_1; break;
                                    case Buildings.Казарма: Village.BonusOfTimeTraining_Barrack += (int)delta_1; break;
                                    case Buildings.Конюшня: Village.BonusOfTimeTraining_Stable += (int)delta_1; break;
                                    case Buildings.Мастерская: Village.BonusOfTime_TrainingInWorkshop += (int)delta_1; break;
                                    case Buildings.Академия://нет уникальных параметров. для бонусов достаточно наличия постройки
                                    break;
                                    case Buildings.Тайник:
                                        if (Account.Folk_Name == Folk.Gaul) delta_1 *= 2;//у галлов тайник в 2 раза больше
                                        Village.Capacity.Stash += (int)(delta_1);
                                    break;
                                    case Buildings.Ратуша:
                                        //ID = 24 РАТУША - теперь имеет 2 уникальных параметра вместо 0 хранящих время
                                        Village.DurationOfHoliday_TownHall += (int)(delta_1);
                                        Village.DurationOfCelebration_TownHall += (int)(delta_2);
                                    break;
                                    case Buildings.Резиденция: Village.ExpansionSlots_Free = 0;
                                        if (Event_lvl >= 10 && Event_lvl <= 19) Village.ExpansionSlots_Free = 1;
                                        else if (Event_lvl == 20) Village.ExpansionSlots_Free = 2;
                                    break;
                                    case Buildings.Дворец: Village.ExpansionSlots_Free = 0;
                                        if (Event_lvl >= 10 && Event_lvl <= 14) Village.ExpansionSlots_Free = 1;
                                        else if (Event_lvl >= 15 && Event_lvl <= 19) Village.ExpansionSlots_Free = 2;
                                        else if (Event_lvl == 20) Village.ExpansionSlots_Free = 3;
                                    break;
                                    case Buildings.Сокровищница:
                                        if (Village.Capacity.Treasury <= Game.Capacity_Basic.Treasury)//это дефолтная вместимость
                                            Village.Capacity.Treasury = 0;
                                        Village.Capacity.Treasury += (int)delta_1;
                                        if (Village.Capacity.Treasury <= 0) Village.Capacity.Treasury = Game.Capacity_Basic.Treasury;
                                    break;
                                    case Buildings.Торговая_палата: Village.BonusOfCapacity_Merchants += (int)delta_1; break;
                                    case Buildings.Большая_казарма: Village.BonusOfTimeTraining_BigBarrack += (int)delta_1; break;
                                    case Buildings.Большая_конюшня: Village.BonusOfTimeTraining_BigStable += (int)delta_1; break;
                                    case Buildings.Городская_стена_Римляне: 
                                        Village.BonusOfProtection_Wall += (int)delta_1;
                                        Village.CompleteDestruction_Wall_CountOfRams += (int)delta_2;
                                    break;
                                    case Buildings.Земляной_вал_Германцы:
                                        Village.BonusOfProtection_Wall += (int)delta_1;
                                        Village.CompleteDestruction_Wall_CountOfRams += (int)delta_2;
                                    break;
                                    case Buildings.Изгородь_Галлы:
                                        Village.BonusOfProtection_Wall += (int)delta_1;
                                        Village.CompleteDestruction_Wall_CountOfRams += (int)delta_2;
                                    break;
                                    case Buildings.Стена_Натары:
                                        Village.BonusOfProtection_Wall += (int)delta_1;
                                        Village.CompleteDestruction_Wall_CountOfRams += (int)delta_2;
                                    break;
                                    case Buildings.Каменотёс: Village.BonusOfStability_Buildings += (int)delta_1; break;
                                    case Buildings.Пивоварня_Германцы: Account.Attack_Bonus += (int)delta_1; break;
                                    case Buildings.Капканщик_Галлы: Village.QuantityOfTraps_Available += (int)delta_1; break;
                                    case Buildings.Таверна: Village.OasisSlots_Free = 0;
                                        if (Event_lvl >= 10 && Event_lvl <= 14) Village.OasisSlots_Free = 1;
                                        else if (Event_lvl >= 15 && Event_lvl == 19) Village.OasisSlots_Free = 2;
                                        else if (Event_lvl == 20) Village.OasisSlots_Free = 3;
                                    break;
                                    case Buildings.Большой_склад: 
                                        if (Village.Capacity.Warehouse <= Game.Capacity_Basic.Warehouse)//это дефолтная вместимость
                                            Village.Capacity.Warehouse = 0;
                                        Village.Capacity.Warehouse += (int)delta_1;
                                        if (Village.Capacity.Warehouse <= 0) Village.Capacity.Warehouse = Game.Capacity_Basic.Warehouse;
                                    break;
                                    case Buildings.Большой_амбар: 
                                        if (Village.Capacity.Barn <= Game.Capacity_Basic.Barn)//это дефолтная вместимость
                                            Village.Capacity.Barn = 0;
                                        Village.Capacity.Barn += (int)delta_1;
                                        if (Village.Capacity.Barn <= 0) Village.Capacity.Barn = Game.Capacity_Basic.Barn;
                                    break;
                                    case Buildings.Водопой_Римляне:
                                        Village.BonusOfTimeTraining_WateringHole += (int)delta_1;
                                        if (Event_lvl >= 10 && Event_lvl <= 14) Village.Bonus_OfCropConsumption_ByMountedScouts_ROMANS = -1;
                                        else if (Event_lvl >= 15 && Event_lvl == 19) Village.Bonus_OfCropConsumption_ByEmperorCavalry_ROMANS = -1;
                                        else if (Event_lvl == 20) Village.Bonus_OfCropConsumption_By_CaesarCavalry_ROMANS = -1;
                                    break;
                                    case Buildings.ПУСТО: break;
                                }

                                //МАНИПУЛЯЦИИ С АККАУНТОМ - ЗАПОЛНЕНИЕ ВСЕХ СУЩЕСТВУЮЩИХ ПОЛЕЙ
                                //если это снос
                                if (Account != null && Village != null) {
                                    if (Game.Event_Stack.Stack[i].Destruction) {
                                        Account.add_EK -= Build_Information.ProductionCulture;
                                        Account.Total_Population -= Build_Information.Consumption;
                                        //Account.Rank;//сделать сортировку игроков по населению. чем больше население, тем выше ранг
                                    } else { //если это строительство
                                        Account.add_EK += Build_Information.ProductionCulture;
                                        Account.Total_Population += Build_Information.Consumption;
                                        //Account.Rank;//сделать сортировку игроков по населению. чем больше население, тем выше ранг
                                    }
                                }

                                DeleteLineInList = true;//разрешить удалить строку в листе стека
                                //если это событие сработало в активной деревне игрока, передаём флаги в таймер циклического обновления контролов
                                Flag_Village_Завершение_Строительства_Сноса = true;
                                Flag_Resources_Завершение_Строительства = true;
                                Flag_Коры_Construction = Game.Event_Stack.Stack[i].Cell_Start;
                                Flag_Slot = Game.Event_Stack.Stack[i].Slot;
                            break;
//################################################ Construction ###################################################
                            default: break;
                        }

                        //bool переменная. true = удалить строку в листе стека, false = оставить строку в покое
                        if (DeleteLineInList) {
                            Game.Event_Stack.Stack.RemoveAt(i);//удаляем строку
                            i--;
                        }
                    }
                }

                for (int i = 0; i < BotList.Count; i++) {
                    if (BotList[i] == null) break;//бота больше нет [?]
                    for (int N = 0; N < BotList[i].VillageList.Count; N++) {
                        if (BotList[i].VillageList[N] == null) break;//деревни больше нет [?]
                        var Village = BotList[i].VillageList[N];//проверяемый бот (в том числе и игрок)
                        //==================== ДОБАВЛЕНИЕ РЕСУРСОВ В ХРАНИЛИЩА ====================
                        //У КАЖДОГО АККАУНТА (ИЛИ ВЫЧИТАНИЕ ЕСЛИ МИНУС ПО ДОБЫЧЕ)
                        var Выработка_В_Час = Village.HourlyProductionResources;
                        var Процент = Village.HourlyProductionResources_PercentOfIncrease;
                        //добавление происходит с учётом всех бонусов % и потребления зерна деревней
                        double quantum_wood_in_sec = (Выработка_В_Час.wood + Выработка_В_Час.wood / 100 * Процент.wood) / 3600;
                        double quantum_clay_in_sec = (Выработка_В_Час.clay + Выработка_В_Час.clay / 100 * Процент.clay) / 3600;
                        double quantum_iron_in_sec = (Выработка_В_Час.iron + Выработка_В_Час.iron / 100 * Процент.iron) / 3600;
                        double quantum_crop_in_sec = (Выработка_В_Час.crop + Выработка_В_Час.crop / 100 * Процент.crop - BotList[i].VillageList[N].Crop_Consumption) / 3600;
                        double quantum_gold_in_sec = (Выработка_В_Час.gold + Выработка_В_Час.gold / 100 * Процент.gold) / 3600;
                        Res add = Village.ResourcesInStorages;
                        add.wood += quantum_wood_in_sec; add.clay += quantum_clay_in_sec; add.iron += quantum_iron_in_sec;
                        add.crop += quantum_crop_in_sec; add.gold += quantum_gold_in_sec;
                        Village.ResourcesInStorages = new Res(add.wood, add.clay, add.iron, add.crop, add.gold);
                        //=========================================================================

                        //========================= ГОЛОД ВОЙСК В ДЕРЕВНЕ =========================
                        //ЕСЛИ crop = 0, то умирает 1 юнит и добавляет своё зерно на склад
                        if (Village.ResourcesInStorages.crop <= 0) { 
                            if (Village.Reinforcements.Count > 0) { int m = 0;
                                //вычисляем строку в листе с максимальным суммарным потреблением зерна войсками
                                int index = 0;//номер строки с максимальным суммарным потреблением зерна войсками
                                int Max = 0;//максимальное количество суммарного потребления зерна войсками
                                for (int j = 0; j < Village.Reinforcements.Count; j++) {
                                    if (Village.Reinforcements[j].Подкрепление != Подкреп.Вход) continue;
                                    int mmm = (int)Village.Reinforcements[j].FolkName * Game.Troops[0].Information.Length;/*Lenght = 10 шт*/
                                    int summ_crop = 0;//сумма поотребления зерна войсками включая героев
                                    for (int k = 0; k < Village.Reinforcements[j].Units.Length - 1; k++)
                                        summ_crop += ((int)Game.Troops[(k + mmm) / 10].Information[(k + mmm) % 10].Consumption * Village.Reinforcements[j].Units[k]);
                                    summ_crop += Village.Reinforcements[j].Units[10] * BotList[i].Hero.Consumption;
                                    if (summ_crop > Max) { Max = summ_crop; m = mmm; index = j; }
                                }
                                //реализация смерти от голода слева направо в массиве Units[]
                                //сперва мрут все входящие подкрепы, затем собственные войска в деревне
                                //герои не мрут, просто при минусе зерно будет на нуле
                                for (int k = 0; k < Village.Reinforcements[index].Units.Length - 1; k++) {
                                    if (Village.Reinforcements[index].Units[k] > 0) {
                                        var Other_Village = Village.Reinforcements[index].LinkVillage;
                                        Point _this = Game.Map.Coord_WorldToMap(Village.Coordinates_World_Travian);
                                        //Point _other = Game.Map.Coord_WorldToMap(Other_Village.Coordinates_World_Travian);
                                        //усыпляем воина
                                        Village.Reinforcements[index].Units[k]--;
                                        Game.Map.Cell[_this.X, _this.Y].AllTroops[k + m]--;
                                        Village.Crop_Consumption -=
                                            ((int)Game.Troops[(k + m) / 10].Information[(k + m) % 10].Consumption);
                                        //усыпляем этого воина и в деревне которая прислала подкреп
                                        //в листе подкрепов ссылка может указывать как на деревню другого аккаунта,
                                        //так и на одну из своих деревень
                                        int index2 = 0;//номер строки в листе у второй дерени
                                        string key = Village.Reinforcements[index].Key;
                                        for (int n = 0; n < Other_Village.Reinforcements.Count; n++) {
                                            if (key == Other_Village.Reinforcements[n].Key) {
                                                Other_Village.Reinforcements[n].Units[k]--;
                                                //ЗАЧЕМ??? из листа удалил и всё. в этих массивах отправленных уже нет
                                                //Game.Map.Cell[_other.X, _other.Y].ValueVillageTroops[k]--;
                                                //Game.Map.Cell[_other.X, _other.Y].ValueAllTroops[k + m]--;
                                                index2 = n;
                                                break;
                                            }
                                        }
                                        //возвращаем зерно в амбар
                                        add.crop += Game.Troops[(k + m) / 10].Information[(k + m) % 10].CostOfTraining.crop;
                                        Village.ResourcesInStorages = new Res(add.wood, add.clay, add.iron, add.crop, add.gold);

                                        bool flag = true;//true = удаляем строку из листа, false = нет, войска ещё есть
                                        //чекаем массив. true - значит войск нет, false - войска есть
                                        for (int n = 0; n < Village.Reinforcements[index].Units.Length; n++)
                                            if (Village.Reinforcements[index].Units[n] > 0) { flag = false; break; }
                                        if (flag) { 
                                            Village.Reinforcements.RemoveAt(index);
                                            Other_Village.Reinforcements.RemoveAt(index2);
                                        }
                                        break;
                                    }
                                }
                            } else { //подкрепов нет, усыпляем родные войска в деревне, кроме героя
                                Point _this = Game.Map.Coord_WorldToMap(Village.Coordinates_World_Travian);
                                int m = (int)BotList[i].Folk_Name * Game.Troops[0].Information.Length;/*Lenght = 10 шт*/
                                for (int k = 0; k < Game.Map.Cell[_this.X, _this.Y].VillageTroops.Length; k++) {
                                    if (Game.Map.Cell[_this.X, _this.Y].VillageTroops[k] > 0) {
                                        Game.Map.Cell[_this.X, _this.Y].VillageTroops[k]--;
                                        Game.Map.Cell[_this.X, _this.Y].AllTroops[k + m]--;
                                        Village.Crop_Consumption -=
                                            ((int)Game.Troops[(k + m) / 10].Information[(k + m) % 10].Consumption);

                                        //возвращаем зерно в амбар
                                        add.crop += Game.Troops[(k + m) / 10].Information[(k + m) % 10].CostOfTraining.crop;
                                        Village.ResourcesInStorages = new Res(add.wood, add.clay, add.iron, add.crop, add.gold);
                                        break;
                                    }
                                }

                            }
                            Flag_Hunger = true;
                            Flag_Коры_Hunger = Game.Map.Coord_WorldToMap(Village.Coordinates_World_Travian);
                        }
                        //=========================================================================
                    }
                    if (BotList[i].IsAlliance() && BotList[i].IsOnline())
                        BotList[i].LinkOnAlliance.GetProperty(BotList[i]).Refresh();
                }
            }
        }

        /// <summary>
        ///     Метод вычисляет текущий наивысший уровень строящейся постройки в переданном слоте с учётом строительства и сноса здания. <br/>
        ///     Метод обращается к стеку событий, если постройка запущена/сносится, берётся текущий максимальный/минимальный уровень из списка. <br/>
        ///     Если постройка не строится (нет в списке), просто берётся текущий уровень из переданного слота. <br/>
        ///     Это нужно чтобы во всплывающей подсказке, в окне повышения уровня постройки и кнопки "добавить в очередь", "снести" работали корректно с учётом запущенных построек.
        /// </summary>
        /// <value> <b> NumberSlot </b> номер слота в деревне. </value>
        /// <returns> Возвращает актуальный уровень постройки который следует инкрементировать. </returns>
        int Актуальный_Уровень_С_Учётом_Строительства_И_Сноса(int NumberSlot) {
            Point p = GAME.Map.Coord_WorldToMap(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian);
            int lvl = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].Level;
            Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID;
            if (Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction) { /*есть строительство*/
                if (ID == Buildings.ПУСТО) ID = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction;
                for (int i = 0; i < GAME.Event_Stack.Stack.Count; i++) {
                    if (GAME.Event_Stack.Stack[i].TypeEvent == Type_Event.Construction)
                        if (GAME.Event_Stack.Stack[i].Cell_Start == p) if (GAME.Event_Stack.Stack[i].ID == ID)
                            if (GAME.Event_Stack.Stack[i].Slot == NumberSlot)
                                //это снос
                                if (GAME.Event_Stack.Stack[i].Destruction) {
                                    if (lvl > GAME.Event_Stack.Stack[i].lvl) lvl = GAME.Event_Stack.Stack[i].lvl;
                                }//это строительство
                                else if (lvl < GAME.Event_Stack.Stack[i].lvl) lvl = GAME.Event_Stack.Stack[i].lvl;
            }}
            return lvl;
        }
    }
}
