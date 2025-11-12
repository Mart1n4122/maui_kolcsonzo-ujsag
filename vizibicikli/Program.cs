using vizibicikli;

var lines = File.ReadAllLines("vizibicikli.csv").Skip(1).Where(l => !string.IsNullOrWhiteSpace(l));
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

foreach (var rent in rents)
{
    var kezdes = new TimeOnly(rent.kora, rent.kperc);
    var vegzes = new TimeOnly(rent.vora, rent.vperc);

    var eltelt = vegzes - kezdes;
    
}