# Schack-visare

Ett script som tar ett schackparti, och låter användaren stega igenom det för att studera dragen.

## Exempel-fil

`Anders mot Patrik;2026-08-07;Anders;Patrik;H2-H4;D7-D5;G1-F3;B8-C6;G2-G4;`

![Screenshot](https://raw.githubusercontent.com/Anders-H/Schack-visare/refs/heads/main/screenshot.jpg "Screenshot")

## Chess Engine

Chess Engine är ett Windows-program som låter användaren skapa och redigera filer för schack-visaren. Du kan även spela upp klassiska schack-partier.

![Screenshot](https://raw.githubusercontent.com/Anders-H/Schack-visare/refs/heads/main/screenshot_engine.jpg "Screenshot")

## Website

[https://ahesselbom.se/chess/](https://ahesselbom.se/chess/)

## Filformatet

Ett parti lagras som en textrad med semikolonseparerade fält: partiets namn,
datum i formatet `yyyy-MM-dd`, vit spelares namn och svart spelares namn,
följt av dragen i spelordning. Varje drag anges med start- och slutruta,
exempelvis `H2-H4`, och varje fält avslutas med semikolon, även det sista draget.
Partiet börjar normalt i standardställningen med vit vid draget, och därefter
växlar dragen mellan vit och svart. Namnfälten får inte innehålla semikolon
eller radbrytningar. Chess Engine kan även ange en annan startställning med
ett valfritt FEN-fält, se avsnittet om FEN-import nedan.

## Specialdrag i filformatet

En passant härleds från det omedelbart föregående draget och kräver inget
extra fält. Promovering kan anges som `E7-E8=Q`, `E7-E8=R`, `E7-E8=B` eller `E7-E8=N`.
Utan suffix blir en bonde som når sista raden automatiskt dam.

Ett avslutat parti kan ha en sista post med `END=WHITE` (vit vann),
`END=BLACK` (svart vann) eller `END=DRAW` (remi), exempelvis
`Parti;2026-09-19;Vit;Svart;E2-E4;END=WHITE;`. Slutposten flyttar ingen pjäs
och måste ligga sist. Filer utan slutpost fungerar som tidigare.

## PGN-import i Chess Engine

Importdialogen för PGN (Portable Game Notation) konverterar ett parti från
standardställningen till schack-visarens filformat. Partinamnet hämtas från
`Event` och `Site`, datumet från `Date` och spelarnamnen från `White` och `Black`.
Dragen i huvudvarianten tolkas från algebraisk notation (SAN), inklusive
rockad, en passant och promovering, och kontrolleras så att de motsvarar
entydiga, lagliga drag. Kommentarer, alternativa varianter och bedömningssymboler
ignoreras. Resultatet från `Result` sparas som slutpost: `1-0` blir `END=WHITE`,
`0-1` blir `END=BLACK` och `1/2-1/2` blir `END=DRAW`. Om taggen saknas används
resultatet i dragtexten. `*` ger ingen slutpost. Importen hanterar ett parti åt
gången; PGN med en egen startställning via FEN/SetUp stöds inte. En fristående
FEN-ställning kan i stället importeras enligt nästa avsnitt.

## FEN-import i Chess Engine

Importdialogen för FEN läser en ställning med alla sex FEN-fält: pjäser,
spelare vid draget, rockadrättigheter, en passant-ruta och båda dragräknarna.
Ställningen kan visas, spelas vidare från och sparas/öppnas i Windows-programmet.
FEN innehåller ingen draghistorik eller spelarmetadata; importerade ställningar
får därför namnet `Imported FEN position`, datum `0001-01-01` och spelarnamn `?`.

Startställningen lagras som ett valfritt `FEN ...`-fält direkt efter spelarnamnen,
före eventuella drag. Exempel med svart vid draget:

`Imported FEN position;0001-01-01;?;?;FEN 4k3/8/8/8/8/8/8/4K3 b - - 17 42;E8-D7;`

Äldre filer utan FEN-fält fortsätter att börja i standardställningen. FEN-fältet
stöds av Chess Engine; webbvisaren har ännu inte stöd för detta tillägg.
Dragräknarna bevaras för startställningen, men används inte för automatisk
remibedömning. Parsern validerar format och grundläggande konsistens, inte att
ställningen kan uppstå genom en fullständig följd av lagliga drag.
