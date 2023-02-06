namespace GameLogica
{
    public class Enums_and_structs {
        //================================================ ENUM ================================================

        //                           ==================================================================
        //                           ========================= enum class Form1 =======================
        //                           ==================================================================
        /// <summary> enum перечисление. Выбор какие слоты обновлять в методе <b>Update_Buttons_Level_Slots:</b> ресурсные или деревенские. <b>None:</b> ничто не обновлять. </summary>
        public enum Slots { Resources, Village, None }
        /// <summary> Перечисление статистических данных. <br/> None = ничего не выводить <br/> Auto = определить согласно нажатой кнопке <br/> Players = вывести статистику игроков <br/> Alliances = вывести статистику альянсов <br/> Villages = вывести статистику деревень <br/> Heroes = вывести статистику героев <br/> Wonders = вывести статистику по чудесам света <br/> </summary>
        public enum TypeStatistics { None, Auto, Players, Alliances, Villages, Heroes, Wonders }
        /// <summary> enum перечисление вариантов открытия окна сообщений (0 = Read/Прочитать; 1 = Write/Написать). </summary>
        public enum Window_Message { Read, Write }

        //                           ==================================================================
        //                           =============== enum class TPlayer / class TVillage ==============
        //                           ==================================================================
        /// <summary> Перечисление всех существующих наций в игре. </summary>
        public enum Folk { Rome, German, Gaul, Nature, Natar, NULL }

        /// <summary> Перечисление всех существующих построек в игре. </summary>
        /// <remarks> Госпиталь [12] - может быть только один в деревне. </remarks>
        public enum Buildings {
            ЧУДО_СВЕТА, Лесопилка, Глиняный_карьер, Железный_рудник, Ферма,
            Лесопильный_завод, Кирпичный_завод, Чугунолитейный_завод, Мукомольная_мельница,
            Пекарня, Склад, Амбар, Госпиталь, Кузница, Арена, Главное_здание, Пункт_сбора,
            Рынок, Посольство, Казарма, Конюшня, Мастерская, Академия, Тайник, Ратуша,
            Резиденция, Дворец, Сокровищница, Торговая_палата, Большая_казарма, Большая_конюшня,
            Городская_стена_Римляне, Земляной_вал_Германцы, Изгородь_Галлы, Стена_Натары, Каменотёс,
            Пивоварня_Германцы, Капканщик_Галлы, Таверна, Большой_склад, Большой_амбар, Водопой_Римляне,
            ПУСТО
        }

        /// <summary> <b> Enum </b> перечисление. </summary>
        /// <remarks>
        ///     Тип деревни: <b> Natars [1]: </b> деревня натар. <b> Other [0]: </b> деревня всех остальных наций. <br/>
        ///     Деревня натар визуально другая. В центре большой слот для строительства ЧУДА СВЕТА, вокруг слоты для вспомогательных построек.
        /// </remarks>
        public enum TypeVillage { Other, Natars }

        /// <summary> Хранит перечисление для вариантов загрузки таблицы культуры. x1 = стандартный сервер, x3 = скоростной x3. <br/> <b>Чем больше значение enum (скорость), тем меньше нужно накопить культуры для основания следующей деревни.</b> </summary>
        public enum SpeedOfServerForCulture { x1, x3 }

        //                           ==================================================================
        //                           ===================== enum class TEvent_Stack ====================
        //                           ==================================================================
        /// <summary> Тип события: передвижение войск, ресурсов, строительство, обучение и т.д. </summary>
        public enum Type_Event {
            /// <summary> 
            ///     КРАСНЫЕ МЕЧИ / входящие перемещения / идущие атаки на игрока [НАПАДЕНИЕ]. <br/>
            ///     ЖЁЛТЫЕ МЕЧИ / исходящие перемещения / отправленные атаки от игрока [НАПАДЕНИЕ]. <br/>
            /// </summary>
            ATTACK,
            /// <summary> 
            ///     КРАСНЫЕ МЕЧИ / входящие перемещения / идущие набеги на игрока [НАБЕГ]. <br/>
            ///     ЖЁЛТЫЕ МЕЧИ / исходящие перемещения / отправленные набеги от игрока [НАБЕГ]. <br/>
            /// </summary>
            RAID,
            /// <summary> 
            ///     ЗЕЛЁНЫЙ ЩИТ / входящие перемещения / идущие войска к игроку (подкреп, возврат с фарма/напа, возврат поселков/говорунов и т.д.). <br/>
            ///     ЖЁЛТЫЙ ЩИТ / исходящие перемещения / отправленные игроком юниты в качестве подкрепления.
            /// </summary>
            REINFORCEMENTS,
            /// <summary> ФИОЛЕТОВЫЕ МЕЧИ / исходящие перемещения / отправленные атаки от игрока на оазис [НАПАДЕНИЕ]. </summary>
            ADVENTURE_ATTACK,
            /// <summary> ФИОЛЕТОВЫЕ МЕЧИ / исходящие перемещения / отправленные набеги от игрока на оазис [НАБЕГ]. </summary>
            ADVENTURE_RAID,
            /// <summary> ПОСЕЛЕНЦЫ / исходящие перемещения / отправленные поселенцы с целью основать поселение. </summary>
            ESTABLISH_A_SETTLEMENT,
            /// <summary> СТРОИТЕЛЬСТВО-СНОС / событие строящейся постройки в ресурсной вкладке или в деревне. </summary>
            Construction,
            /// <summary> Ничто из перечисленного. </summary>
            None
        }

        //                           ==================================================================
        //                           ======================= enum class TReport =======================
        //                           ==================================================================
        /// <summary> Типы событий для вкладки ОТЧЁТЫ. </summary>
        /// <remarks> Количество награбленного на победу или проигрыш не влияет. </remarks>
        public enum Type_Report {
            /// <summary> Как нападающий, вы выиграли без потерь. Папка пиктограмм: <b>ico/move</b> </summary>
            Attack_Win_GREEN,
            /// <summary> Как нападающий, вы выиграли, но с потерями. Папка пиктограмм: <b>ico/move</b> </summary>
            Attack_Losses_YELLOW,
            /// <summary> Как нападающий, вы проиграли (никто не выжил). Папка пиктограмм: <b>ico/move</b> </summary>
            Attack_Dead_RED,
            /// <summary> Как защитник, вы выиграли без потерь. Папка пиктограмм: <b>ico/move</b> </summary>
            Defend_Win_GREEN,
            /// <summary> Как защитник, вы выиграли, но с потерями. Папка пиктограмм: <b>ico/move</b> </summary>
            Defend_Losses_YELLOW,
            /// <summary> Как защитник, вы проиграли (никто не выжил). Папка пиктограмм: <b>ico/move</b> </summary>
            Defend_Dead_RED,
            /// <summary> Как защитник, вы выиграли без потерь (серый означает что у аккаунта не было войск). Папка пиктограмм: <b>ico/move</b> </summary>
            Defend_Win_GRAY,
            /// <summary> Прибыло подкрепление. Папка пиктограмм: <b>ico/move</b> </summary>
            Reinforcement_Arrived,
            /// <summary> Различное, например нападение Натар. Папка пиктограмм: <b>ico/other</b> </summary>
            Miscellaneous,
            /// <summary> Отчёт о приключении. Папка пиктограмм: <b>ico/other</b> </summary>
            Adventure,
            /// <summary> Были пойманы животные. Папка пиктограмм: <b>ico/other</b> </summary>
            Animals_Caught,
            /// <summary> Разведывательная операция прошла успешно и не была раскрыта. Папка пиктограмм: <b>ico/other</b> </summary>
            Won_Scout_Attacker_GREEN,
            /// <summary> Разведывательная операция прошла успешно, но с потерями. Папка пиктограмм: <b>ico/other</b> </summary>
            Won_Scout_Attacker_YELLOW,
            /// <summary> Разведывательная операция не удалась (никто не выжил). Папка пиктограмм: <b>ico/other</b> </summary>
            Lost_Scout_Attacker_RED,
            /// <summary> Разведывательная операция успешно отражена. Папка пиктограмм: <b>ico/other</b> </summary>
            Won_Scout_Defender_GREEN,
            /// <summary> Разведывательная операция не была отражена. Папка пиктограмм: <b>ico/other</b> </summary>
            Lost_Scout_Defender_YELLOW,
            /// <summary> Торговец доставил в основном древесину. Папка пиктограмм: <b>ico/trading</b> </summary>
            Mostly_wood,
            /// <summary> Торговец доставил в основном глину. Папка пиктограмм: <b>ico/trading</b> </summary>
            Mostly_clay,
            /// <summary> Торговец доставил в основном железо. Папка пиктограмм: <b>ico/trading</b> </summary>
            Mostly_iron,
            /// <summary> Торговец доставил в основном зерно. Папка пиктограмм: <b>ico/trading</b> </summary>
            Mostly_crop,
            /// <summary> Торговец доставил в основном золото. Папка пиктограмм: <b>ico/trading</b> </summary>
            Mostly_gold,
            /// <summary> Ваш подкреп. в [деревня] был атакован (пиктограммы щитов). </summary>
            Reinforcements_Attacked,
            /// <summary> Была основана новая деревня. Папка пиктограмм: <b>ico/move</b> </summary>
            SettlersCreateNewVillage
        }

        //                           ===================================================================
        //                           ======================= enum class TMessage =======================
        //                           ===================================================================
        /// <summary> Тип сообщений для вкладки СООБЩЕНИЯ. </summary>
        public enum Type_Message { 
            /// <summary> Входящее сообщение. </summary>
            Incoming,
            /// <summary> Отправленное сообщение. </summary>
            Outgoing
        }

        //                           ===================================================================
        //                           =============== enum class Reinforcements_Of_Troops ===============
        //                           ===================================================================
        /// <summary> Тип подкрепления (Входящий/исходящий). Вход = 0, Исход = 1. </summary>
        public enum Подкреп { Вход, Исход }

        /// <summary> Тип передвижений. None = передвижений нет. </summary>
        public enum Type_Movement { None, Возвращение_войск, Подкрепление }

        /// <summary>
        ///     Хранит варианты освобождаемых войск из капканов галлов: <br/> 
        ///     [0] None (default); [1] освобождены только свои войска; [2] освобождены только союзные войска; [3] освобождены как свои, так и союзные войска.
        /// </summary>
        public enum Traps { None, My_Troops, Ally_Troops, My_And_Ally_Troops }

        //                           ==================================================================
        //                           ========================= enum class Map =========================
        //                           ==================================================================
        /// <summary>
        ///     Хранит типы ячеек на карте по комбинациям ресурсных полей. <br/>
        ///     Диапазон: [0..3] = 4. <br/>
        ///     Пример: _4_4_4_6 = лесопилок 4, глиняных карьеров 4, железных рудников 4, зерновых полей 6. <br/>
        ///     _0_0_0_0 - поле для оазисов, озёр, непроходимых местностей, декор и т.д.
        /// </summary>
        public enum TypeCell { _0_0_0_0, _4_4_4_6, _3_4_5_6, _3_5_4_6, _3_4_4_7, _5_4_3_6, _4_3_5_6, _4_3_4_7,
                               _4_4_3_7, _3_3_3_9, _1_1_1_15 }

        /// <summary>
        ///     <b>enum</b> перечисление состояний размеров деревни. Всего их четыре: <br/>
        ///     <b> Tiny (крошечный): </b> население деревни [0..299] <br/>
        ///     <b> Small (маленький): </b> население деревни [300..599] <br/>
        ///     <b> Medium (средний): </b> население деревни [600..899] <br/>
        ///     <b> Large (большой): </b> население деревни [900..MAX] <br/>
        /// </summary>
        public enum SizeVillage { Tiny = 0, Small = 300, Medium = 600, Large = 900 }
        /// <summary> <b>enum</b> перечисление состояний ячеек карты. </summary>
        public enum Cell_ID {
            /// <summary> Пустая чейка. </summary>
            Null,                           //ЕСТЬ
            /// <summary> Дикое поле. Ячейка пригодная для основания деревни. </summary>
            Wild_Field,                     //ЕСТЬ
            /// <summary> Озеро / Пруд / Вода. </summary>
            Water,                          //ЕСТЬ
            /// <summary> Лес. </summary>
            Forest,                         //ЕСТЬ
            /// <summary> Горы. </summary>
            Mountains,                      //ЕСТЬ
            /// <summary> Деревня аккаунта с населением [0..299] ед. (если не менял enum SizeVillage) </summary>
            Village_Tiny,                  //ЕСТЬ
            /// <summary> Деревня аккаунта с населением [300..599] ед. (если не менял enum SizeVillage) </summary>
            Village_Small,                 //ЕСТЬ
            /// <summary> Деревня аккаунта с населением [600..899] ед. (если не менял enum SizeVillage) </summary>
            Village_Medium,                //ЕСТЬ
            /// <summary> Деревня аккаунта с населением [900..MAX] ед. (если не менял enum SizeVillage) </summary>
            Village_Large,                 //ЕСТЬ
            /// <summary> Оазис - древесина 25% в час. </summary>
            Oasis_wood25,                   //ЕСТЬ
            /// <summary> Оазис - древесина 50% в час. </summary>
            Oasis_wood50,                   //ЕСТЬ
            /// <summary> Оазис - древесина 25% в час, зерно 25% в час. </summary>
            Oasis_wood25_crop25,            //ЕСТЬ
            /// <summary> Оазис - глина 25% в час. </summary>
            Oasis_clay25,                   //ЕСТЬ
            /// <summary> Оазис - глина 50% в час. </summary>
            Oasis_clay50,                   //ЕСТЬ
            /// <summary> Оазис - глина 25% в час, зерно 25% в час. </summary>
            Oasis_clay25_crop25,            //ЕСТЬ
            /// <summary> Оазис - железо 25% в час. </summary>
            Oasis_iron25,                   //ЕСТЬ
            /// <summary> Оазис - железо 50% в час. </summary>
            Oasis_iron50,                   //ЕСТЬ
            /// <summary> Оазис - железо 25% в час, зерно 25% в час. </summary>
            Oasis_iron25_crop25,            //ЕСТЬ
            /// <summary> Оазис - зерно 25% в час. </summary>
            Oasis_crop25,                   //ЕСТЬ
            /// <summary> Оазис - зерно 50% в час. </summary>
            Oasis_crop50,                   //ЕСТЬ
        }
        //================================================ ENUM ================================================

        //=============================================== STRUCT ===============================================
        /// <summary> Структура типа <b>int</b> для базовых параметров разных наций. </summary>
        public struct BasicParameterOfNation {
            public int Romans; public int Gauls; public int Germans;
            public BasicParameterOfNation(int Romans, int Gauls, int Germans) {
                this.Romans = Romans; this.Gauls = Gauls; this.Germans = Germans;
            }
        }

        /// <summary> Структура типа <b>double</b> для ресурсов. Древисина, глина, железо, зерно, золото. </summary>
        public struct Res {
            public double wood; public double clay; public double iron; public double crop; public double gold;
            public Res(double wood, double clay, double iron, double crop, double gold) {
                this.wood = wood; this.clay = clay; this.iron = iron; this.crop = crop; this.gold = gold;
            }
        }

        /// <summary> Структура типа <b>int</b> - вместимость построек у которых есть свойство ВМЕСТИМОСТЬ. Например: склад, амбар, казна, посольство, тайник и т.д. </summary>
        public struct sCapacity {
            /// <summary> Суммарная вместимость всех складов </summary>
            public int Warehouse;
            /// <summary> Суммарная вместимость всех амбаров (зернохранилища) </summary>
            public int Barn;
            /// <summary> Capacity казны (золото) </summary>
            public int Treasury;
            /// <summary> Capacity посольства </summary>
            public int Embassy;
            /// <summary> Суммарная вместимость всех тайников </summary>
            public int Stash;
            public sCapacity(int Склад, int Амбар, int Казна, int Посольство, int Тайник) {
                Warehouse = Склад; Barn = Амбар; Treasury = Казна; Embassy = Посольство; Stash = Тайник;
            }
        }

        /// <summary> Структура цвета <b>HSV.</b> <br/> <b>(H) <paramref name="H"/>ue</b> - Оттенок [0..360°]; <br/> <b>(S) <paramref name="S"/>aturation</b> - Насыщение (от 0.0 до 1.0); <br/> <b>(V) <paramref name="V"/>alue</b> - яркость (от 0.0 до 1.0); <br/> </summary>
        /// <remarks> Конвертация <b>ColorToHSV / HSVToColor</b> описаны в классе <b>UFO.Convert</b> в качестве расширений <b>(extensions)</b>. </remarks>
        public struct HSV {
            /// <summary> Цветовой тон (например, красный, зелёный или синий). Варьируется в пределах [0..360°]. </summary>
            private double _hue;
            /// <summary> <inheritdoc cref="_hue"/> </summary> <remarks> Осуществляется проверка диапазона. </remarks>
            public double Hue { get { return _hue; } set { _hue = value < 0 ? 0 : value > 360 ? 360 : value; }}
            /// <summary> Насыщенность. Диапазон от 0.0 до 1.0, где 0.0 представляет серость, а 1.0 представляет насыщенность. </summary>
            private double _saturation;
            /// <summary> <inheritdoc cref="_saturation"/> </summary> <remarks> <inheritdoc cref="Hue"/> </remarks>
            public double Saturation { get { return _saturation; } set { _saturation = value < 0 ? 0 : value > 1 ? 1 : value; } }
            /// <summary> Яркость. Диапазон от 0.0 до 1.0, где 0.0 представляет чёрное, а 1.0 - белое. </summary>
            private double _value;
            /// <summary> <inheritdoc cref="_value"/> </summary> <remarks> <inheritdoc cref="Hue"/> </remarks>
            public double Value { get { return _value; } set { _value = value < 0 ? 0 : value > 1 ? 1 : value; } }
            public HSV(double H, double S, double V) { _hue = H; _saturation = S; _value = V; }
        }
        //=============================================== STRUCT ===============================================
    }
}
