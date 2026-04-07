using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization;

namespace BanEnemyMod.BanEnemyModCode.Localization;

internal static class BanEnemyText
{
    private static readonly Dictionary<string, Dictionary<string, string>> Tables = new(StringComparer.OrdinalIgnoreCase)
    {
        ["eng"] = new()
        {
            ["button.open"] = "Ban Enemy",
            ["title"] = "Ban Enemy",
            ["close"] = "Close",
            ["search.placeholder"] = "Search by encounter, act, category, or monsters...",
            ["select_all"] = "Select All",
            ["deselect_all"] = "Deselect All",
            ["preview.title"] = "Encounter Preview",
            ["preview.empty"] = "Hover an encounter to preview its monsters.",
            ["column.encounter"] = "Encounter",
            ["column.monsters"] = "Monsters",
            ["act"] = "Act {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Elite",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Changes save immediately and will be used for the next run.",
            ["status.client"] = "Multiplayer clients use the host snapshot when the run begins.",
            ["status.readonly"] = "Run snapshot active. Configuration is read-only.",
            ["count"] = "Press B to toggle. Showing {0} encounter groups. Disabled: {1}.",
            ["rule_hint"] = "Rule: each map and encounter type must keep at least one encounter enabled.",
            ["warn.group_min"] = "{0} {1} must keep at least one encounter enabled.",
            ["warn.group_generic"] = "Each map and encounter type must keep at least one encounter enabled."
        },
        ["zhs"] = new()
        {
            ["button.open"] = "禁用敌人",
            ["title"] = "禁用敌人",
            ["close"] = "关闭",
            ["search.placeholder"] = "按战斗、层数、类别或怪物搜索……",
            ["select_all"] = "全选",
            ["deselect_all"] = "全不选",
            ["preview.title"] = "战斗预览",
            ["preview.empty"] = "将鼠标悬停在战斗上以预览其中的怪物。",
            ["column.encounter"] = "战斗",
            ["column.monsters"] = "怪物",
            ["category.normal"] = "小怪",
            ["category.elite"] = "精英",
            ["category.boss"] = "Boss",
            ["status.editable"] = "修改会立即保存，并用于下一局游戏。",
            ["status.client"] = "多人模式客户端会在开局时使用房主快照。",
            ["status.readonly"] = "本局快照已生效，当前配置为只读。",
            ["count"] = "按 B 键开关。当前显示 {0} 个战斗关卡，已禁用 {1} 个。",
            ["rule_hint"] = "规则：每张地图的每种战斗类型都至少要保留一个，无法全部取消。",
            ["warn.group_min"] = "{0} 的{1}至少要保留一个战斗。",
            ["warn.group_generic"] = "每张地图的每种战斗类型都至少要保留一个。"
        },
        ["deu"] = new()
        {
            ["button.open"] = "Gegner sperren",
            ["title"] = "Gegner sperren",
            ["close"] = "Schließen",
            ["search.placeholder"] = "Nach Begegnung, Akt, Kategorie oder Monstern suchen...",
            ["select_all"] = "Alle auswählen",
            ["preview.title"] = "Begegnungsvorschau",
            ["preview.empty"] = "Bewege den Mauszeiger über eine Begegnung, um ihre Monster anzuzeigen.",
            ["column.encounter"] = "Begegnung",
            ["column.monsters"] = "Monster",
            ["act"] = "Akt {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Elite",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Änderungen werden sofort gespeichert und für den nächsten Lauf verwendet.",
            ["status.client"] = "Mehrspieler-Clients verwenden beim Start den Host-Schnappschuss.",
            ["status.readonly"] = "Lauf-Schnappschuss aktiv. Konfiguration ist schreibgeschützt.",
            ["count"] = "Mit B umschalten. {0} Begegnungen sichtbar. Deaktiviert: {1}.",
            ["warn.group_min"] = "In {0} {1} muss mindestens eine Begegnung aktiviert bleiben.",
            ["warn.group_generic"] = "Für jeden Akt und Typ muss mindestens eine Begegnung aktiviert bleiben."
        },
        ["esp"] = new()
        {
            ["button.open"] = "Bloquear enemigos",
            ["title"] = "Bloquear enemigos",
            ["close"] = "Cerrar",
            ["search.placeholder"] = "Buscar por encuentro, acto, categoría o monstruos...",
            ["select_all"] = "Seleccionar todo",
            ["preview.title"] = "Vista previa del encuentro",
            ["preview.empty"] = "Pasa el cursor sobre un encuentro para ver sus monstruos.",
            ["column.encounter"] = "Encuentro",
            ["column.monsters"] = "Monstruos",
            ["act"] = "Acto {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Élite",
            ["category.boss"] = "Jefe",
            ["status.editable"] = "Los cambios se guardan al instante y se usarán en la próxima partida.",
            ["status.client"] = "Los clientes multijugador usan la instantánea del anfitrión al comenzar.",
            ["status.readonly"] = "Instantánea de partida activa. La configuración es de solo lectura.",
            ["count"] = "Pulsa B para alternar. Mostrando {0} encuentros. Desactivados: {1}.",
            ["warn.group_min"] = "{0} {1} debe conservar al menos un encuentro habilitado.",
            ["warn.group_generic"] = "Cada acto y tipo de encuentro debe conservar al menos uno habilitado."
        },
        ["fra"] = new()
        {
            ["button.open"] = "Bannir des ennemis",
            ["title"] = "Bannir des ennemis",
            ["close"] = "Fermer",
            ["search.placeholder"] = "Rechercher par rencontre, acte, catégorie ou monstres...",
            ["select_all"] = "Tout sélectionner",
            ["preview.title"] = "Aperçu de la rencontre",
            ["preview.empty"] = "Survolez une rencontre pour afficher ses monstres.",
            ["column.encounter"] = "Rencontre",
            ["column.monsters"] = "Monstres",
            ["act"] = "Acte {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Élite",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Les changements sont enregistrés immédiatement et utilisés pour la prochaine partie.",
            ["status.client"] = "Les clients multijoueur utilisent l’instantané de l’hôte au lancement.",
            ["status.readonly"] = "Instantané de partie actif. Configuration en lecture seule.",
            ["count"] = "Appuyez sur B pour afficher/masquer. {0} rencontres affichées. Désactivées : {1}.",
            ["warn.group_min"] = "{0} {1} doit conserver au moins une rencontre activée.",
            ["warn.group_generic"] = "Chaque acte et type de rencontre doit en conserver au moins une."
        },
        ["ita"] = new()
        {
            ["button.open"] = "Bandisci nemici",
            ["title"] = "Bandisci nemici",
            ["close"] = "Chiudi",
            ["search.placeholder"] = "Cerca per incontro, atto, categoria o mostri...",
            ["select_all"] = "Seleziona tutto",
            ["preview.title"] = "Anteprima incontro",
            ["preview.empty"] = "Passa il mouse su un incontro per vedere i suoi mostri.",
            ["column.encounter"] = "Incontro",
            ["column.monsters"] = "Mostri",
            ["act"] = "Atto {0}",
            ["category.normal"] = "Normale",
            ["category.elite"] = "Elite",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Le modifiche vengono salvate subito e usate nella prossima partita.",
            ["status.client"] = "I client multigiocatore usano l’istantanea dell’host all’avvio.",
            ["status.readonly"] = "Istantanea della run attiva. Configurazione in sola lettura.",
            ["count"] = "Premi B per mostrare/nascondere. Incontri visibili: {0}. Disabilitati: {1}.",
            ["warn.group_min"] = "{0} {1} deve mantenere almeno un incontro attivo.",
            ["warn.group_generic"] = "Ogni atto e tipo di incontro deve mantenere almeno un incontro attivo."
        },
        ["jpn"] = new()
        {
            ["button.open"] = "敵を除外",
            ["title"] = "敵を除外",
            ["close"] = "閉じる",
            ["search.placeholder"] = "遭遇、Act、分類、モンスター名で検索...",
            ["select_all"] = "すべて選択",
            ["preview.title"] = "遭遇プレビュー",
            ["preview.empty"] = "遭遇にカーソルを合わせるとモンスターを表示します。",
            ["column.encounter"] = "遭遇",
            ["column.monsters"] = "モンスター",
            ["act"] = "Act {0}",
            ["category.normal"] = "通常",
            ["category.elite"] = "エリート",
            ["category.boss"] = "ボス",
            ["status.editable"] = "変更は即座に保存され、次のランに適用されます。",
            ["status.client"] = "マルチプレイのクライアントは開始時にホストのスナップショットを使用します。",
            ["status.readonly"] = "ランのスナップショットが有効です。設定は読み取り専用です。",
            ["count"] = "Bで切り替え。表示中の遭遇: {0}、無効: {1}。",
            ["warn.group_min"] = "{0} の {1} は少なくとも1つ有効のままにする必要があります。",
            ["warn.group_generic"] = "各Act・各遭遇タイプで少なくとも1つは有効にする必要があります。"
        },
        ["kor"] = new()
        {
            ["button.open"] = "적 금지",
            ["title"] = "적 금지",
            ["close"] = "닫기",
            ["search.placeholder"] = "전투, 액트, 분류, 몬스터로 검색...",
            ["select_all"] = "모두 선택",
            ["preview.title"] = "전투 미리보기",
            ["preview.empty"] = "전투 위에 마우스를 올리면 몬스터를 미리 봅니다.",
            ["column.encounter"] = "전투",
            ["column.monsters"] = "몬스터",
            ["act"] = "액트 {0}",
            ["category.normal"] = "일반",
            ["category.elite"] = "엘리트",
            ["category.boss"] = "보스",
            ["status.editable"] = "변경 사항은 즉시 저장되며 다음 런에 적용됩니다.",
            ["status.client"] = "멀티플레이 클라이언트는 시작 시 호스트 스냅샷을 사용합니다.",
            ["status.readonly"] = "런 스냅샷이 활성화되었습니다. 설정은 읽기 전용입니다.",
            ["count"] = "B 키로 토글. 표시 중인 전투 {0}개, 비활성화 {1}개.",
            ["warn.group_min"] = "{0} {1}에는 최소 하나의 전투가 남아 있어야 합니다.",
            ["warn.group_generic"] = "각 액트와 전투 유형마다 최소 하나는 남아 있어야 합니다."
        },
        ["pol"] = new()
        {
            ["button.open"] = "Zablokuj wrogów",
            ["title"] = "Zablokuj wrogów",
            ["close"] = "Zamknij",
            ["search.placeholder"] = "Szukaj po starciu, akcie, kategorii lub potworach...",
            ["select_all"] = "Zaznacz wszystko",
            ["preview.title"] = "Podgląd starcia",
            ["preview.empty"] = "Najedź na starcie, aby zobaczyć potwory.",
            ["column.encounter"] = "Starcie",
            ["column.monsters"] = "Potwory",
            ["act"] = "Akt {0}",
            ["category.normal"] = "Normalne",
            ["category.elite"] = "Elita",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Zmiany zapisują się od razu i zostaną użyte w następnym biegu.",
            ["status.client"] = "Klienci multiplayer użyją migawki hosta przy starcie.",
            ["status.readonly"] = "Migawka biegu aktywna. Konfiguracja tylko do odczytu.",
            ["count"] = "Naciśnij B, aby przełączyć. Widoczne starcia: {0}. Wyłączone: {1}.",
            ["warn.group_min"] = "{0} {1} musi zachować co najmniej jedno włączone starcie.",
            ["warn.group_generic"] = "Każdy akt i typ starcia musi zachować przynajmniej jedno włączone starcie."
        },
        ["ptb"] = new()
        {
            ["button.open"] = "Banir inimigos",
            ["title"] = "Banir inimigos",
            ["close"] = "Fechar",
            ["search.placeholder"] = "Buscar por encontro, ato, categoria ou monstros...",
            ["select_all"] = "Selecionar tudo",
            ["preview.title"] = "Prévia do encontro",
            ["preview.empty"] = "Passe o cursor sobre um encontro para ver seus monstros.",
            ["column.encounter"] = "Encontro",
            ["column.monsters"] = "Monstros",
            ["act"] = "Ato {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Elite",
            ["category.boss"] = "Chefe",
            ["status.editable"] = "As mudanças são salvas imediatamente e usadas na próxima partida.",
            ["status.client"] = "Clientes multijogador usam o instantâneo do host ao iniciar.",
            ["status.readonly"] = "Instantâneo da run ativo. Configuração somente leitura.",
            ["count"] = "Pressione B para alternar. Mostrando {0} encontros. Desativados: {1}.",
            ["warn.group_min"] = "{0} {1} precisa manter ao menos um encontro habilitado.",
            ["warn.group_generic"] = "Cada ato e tipo de encontro precisa manter ao menos um habilitado."
        },
        ["rus"] = new()
        {
            ["button.open"] = "Запретить врагов",
            ["title"] = "Запретить врагов",
            ["close"] = "Закрыть",
            ["search.placeholder"] = "Поиск по встрече, акту, категории или монстрам...",
            ["select_all"] = "Выбрать всё",
            ["preview.title"] = "Предпросмотр встречи",
            ["preview.empty"] = "Наведите курсор на встречу, чтобы увидеть монстров.",
            ["column.encounter"] = "Встреча",
            ["column.monsters"] = "Монстры",
            ["act"] = "Акт {0}",
            ["category.normal"] = "Обычный",
            ["category.elite"] = "Элита",
            ["category.boss"] = "Босс",
            ["status.editable"] = "Изменения сохраняются сразу и будут использованы в следующем забеге.",
            ["status.client"] = "Клиенты в мультиплеере используют снимок хоста при старте.",
            ["status.readonly"] = "Снимок забега активен. Конфигурация только для чтения.",
            ["count"] = "Нажмите B для переключения. Показано встреч: {0}. Отключено: {1}.",
            ["warn.group_min"] = "В группе {0} {1} должна остаться хотя бы одна включённая встреча.",
            ["warn.group_generic"] = "Для каждого акта и типа встречи должна остаться хотя бы одна включённая встреча."
        },
        ["spa"] = new()
        {
            ["button.open"] = "Bloquear enemigos",
            ["title"] = "Bloquear enemigos",
            ["close"] = "Cerrar",
            ["search.placeholder"] = "Buscar por encuentro, acto, categoría o monstruos...",
            ["select_all"] = "Seleccionar todo",
            ["preview.title"] = "Vista previa del encuentro",
            ["preview.empty"] = "Pasa el cursor sobre un encuentro para ver sus monstruos.",
            ["column.encounter"] = "Encuentro",
            ["column.monsters"] = "Monstruos",
            ["act"] = "Acto {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Élite",
            ["category.boss"] = "Jefe",
            ["status.editable"] = "Los cambios se guardan al instante y se usarán en la próxima partida.",
            ["status.client"] = "Los clientes multijugador usan la instantánea del anfitrión al comenzar.",
            ["status.readonly"] = "Instantánea de partida activa. La configuración es de solo lectura.",
            ["count"] = "Pulsa B para alternar. Mostrando {0} encuentros. Desactivados: {1}.",
            ["warn.group_min"] = "{0} {1} debe conservar al menos un encuentro habilitado.",
            ["warn.group_generic"] = "Cada acto y tipo de encuentro debe conservar al menos uno habilitado."
        },
        ["tha"] = new()
        {
            ["button.open"] = "แบนศัตรู",
            ["title"] = "แบนศัตรู",
            ["close"] = "ปิด",
            ["search.placeholder"] = "ค้นหาตามการต่อสู้ แอคต์ ประเภท หรือมอนสเตอร์...",
            ["select_all"] = "เลือกทั้งหมด",
            ["preview.title"] = "ตัวอย่างการต่อสู้",
            ["preview.empty"] = "เลื่อนเมาส์ไปบนการต่อสู้เพื่อดูมอนสเตอร์",
            ["column.encounter"] = "การต่อสู้",
            ["column.monsters"] = "มอนสเตอร์",
            ["act"] = "แอคต์ {0}",
            ["category.normal"] = "ปกติ",
            ["category.elite"] = "อีลิต",
            ["category.boss"] = "บอส",
            ["status.editable"] = "การเปลี่ยนแปลงจะถูกบันทึกทันทีและใช้กับรันถัดไป",
            ["status.client"] = "ไคลเอนต์หลายผู้เล่นจะใช้สแนปช็อตของโฮสต์เมื่อเริ่มรัน",
            ["status.readonly"] = "สแนปช็อตของรันกำลังใช้งานอยู่ การตั้งค่านี้เป็นแบบอ่านอย่างเดียว",
            ["count"] = "กด B เพื่อสลับ แสดง {0} การต่อสู้ ปิดใช้งาน {1}",
            ["warn.group_min"] = "{0} {1} ต้องเหลือการต่อสู้อย่างน้อยหนึ่งรายการ",
            ["warn.group_generic"] = "แต่ละแอคต์และแต่ละประเภทต้องเหลือการต่อสู้อย่างน้อยหนึ่งรายการ"
        },
        ["tur"] = new()
        {
            ["button.open"] = "Düşmanları yasakla",
            ["title"] = "Düşmanları yasakla",
            ["close"] = "Kapat",
            ["search.placeholder"] = "Karşılaşma, perde, kategori veya canavar adına göre ara...",
            ["select_all"] = "Tümünü seç",
            ["preview.title"] = "Karşılaşma önizlemesi",
            ["preview.empty"] = "Canavarları görmek için bir karşılaşmanın üzerine gel.",
            ["column.encounter"] = "Karşılaşma",
            ["column.monsters"] = "Canavarlar",
            ["act"] = "Perde {0}",
            ["category.normal"] = "Normal",
            ["category.elite"] = "Elit",
            ["category.boss"] = "Boss",
            ["status.editable"] = "Değişiklikler hemen kaydedilir ve bir sonraki koşuda kullanılır.",
            ["status.client"] = "Çok oyunculu istemciler başlangıçta sunucu anlık görüntüsünü kullanır.",
            ["status.readonly"] = "Koşu anlık görüntüsü etkin. Yapılandırma salt okunur.",
            ["count"] = "Aç/kapat için B'ye bas. Gösterilen karşılaşma: {0}. Devre dışı: {1}.",
            ["warn.group_min"] = "{0} {1} için en az bir karşılaşma etkin kalmalıdır.",
            ["warn.group_generic"] = "Her perde ve karşılaşma türü için en az bir karşılaşma etkin kalmalıdır."
        }
    };

    public static string Get(string key, params object[] args)
    {
        string language = LocManager.Instance?.Language ?? "eng";
        if (!Tables.TryGetValue(language, out Dictionary<string, string>? table))
        {
            table = Tables["eng"];
        }

        if (!table.TryGetValue(key, out string? value))
        {
            value = Tables["eng"].TryGetValue(key, out string? fallback) ? fallback : key;
        }

        return args.Length == 0 ? value : string.Format(value, args);
    }
}
