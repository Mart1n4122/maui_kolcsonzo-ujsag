using vizibicikli;

var lines = File.ReadAllLines("vizibicikli.csv")
    .Skip(1)
    .Where(l => !string.IsNullOrWhiteSpace(l));
var rents = new List<Rent>();

foreach (var line in lines)
{
    var columns = line.Split(',').Select(s => s.Trim()).ToArray();
    var rent = new Rent
    {
        nev = columns[0],
        azonosito = columns[1],
        kora = int.Parse(columns[2]),
        kperc = int.Parse(columns[3]),
        vora = int.Parse(columns[4]),
        vperc = int.Parse(columns[5])
    };
    rents.Add(rent);
}

Console.WriteLine("CSV adat:");
foreach (var rent in rents)
{
    Console.WriteLine($"{rent.nev} {rent.azonosito} {rent.kora} {rent.kperc} {rent.vora} {rent.vperc}");
}

const int pricePerBlock = 1000;
const int minutesPerBlock = 30;

var groups = rents.GroupBy(r => r.azonosito).OrderBy(g => g.Key);

Console.WriteLine();
Console.WriteLine("Összesítés csoportonként:");

foreach (var group in groups)
{
    TimeSpan groupTotalDuration = TimeSpan.Zero;
    int groupTotalFee = 0;
    int entries = 0;

    foreach (var rent in group)
    {
        var kezdes = new TimeOnly(rent.kora, rent.kperc);
        var vegzes = new TimeOnly(rent.vora, rent.vperc);

        if (vegzes < kezdes)
        {
            vegzes = vegzes.AddHours(24);
        }

        var eltelt = vegzes - kezdes;
        groupTotalDuration += eltelt;

        var blocks = (int)Math.Ceiling(eltelt.TotalMinutes / minutesPerBlock);
        var fee = blocks * pricePerBlock;
        groupTotalFee += fee;

        entries++;
    }

    var hours = (int)groupTotalDuration.TotalHours;
    var minutes = groupTotalDuration.Minutes;

    Console.WriteLine();
    Console.WriteLine($"Csoport: {group.Key}");
    Console.WriteLine($"  Bejegyzések száma: {entries}");
    Console.WriteLine($"  Összes idő: {hours} óra {minutes} perc");
    Console.WriteLine($"  Összes fizetendő: {groupTotalFee} Ft");
}