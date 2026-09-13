# PGN-import

PgnParser.Parse konverterar ett parti från standardstartpositionen till
ChessEngines semikolonseparerade format. Event och Site bildar partinamnet,
Date konverteras från yyyy.MM.dd till yyyy-MM-dd och White/Black blir spelarnamn.
Saknade spelarnamn blir ?. Okända datumdelar ersätts med 0001/01/01 och
rapporteras i message även när Parse lyckas.

Parsern följer huvudvarianten, ignorerar kommentarer, alternativa varianter
(även nästlade), NAG och schack-/matt-/bedömningssuffix. Den använder ChessRules
för att lösa SAN till ett entydigt lagligt drag, inklusive rockad.
Schack- och mattsuffix verifieras inte separat. Resultatet lagras inte eftersom
målformatet saknar ett sådant fält.

En passant och promovering till Q/R/B/N stöds. FEN/SetUp avvisas eftersom
uppspelningen kräver standardstartpositionen. Flera partier i samma indata avvisas.
Metadata med semikolon eller radbrytningar avvisas av GameFileFormat.
Vid fel returneras false, ett felmeddelande och tom contents.

Kör testerna med:

    dotnet run --project tests/PgnParser.Tests

Tester täcker bland annat konvertering och återinläsning via GameParser,
båda rockaderna, slag, SAN-tvetydighet, kommentarer, datum och felhantering.

Formatreferens: https://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm
