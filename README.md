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

## Specialdrag

En passant härleds från det omedelbart föregående draget och kräver inget
extra fält. Promovering kan anges som E7-E8=Q, E7-E8=R, E7-E8=B eller E7-E8=N.
Utan suffix blir en bonde som når sista raden automatiskt dam, för kompatibilitet
med äldre filer. PGN-importen bevarar det angivna pjäsvalet. Vid manuell
registrering visas ett pjäsval med dam förvald. Windows-programmet och
webbvisaren spelar upp båda specialdragen.

Tester: dotnet run --project tests/PgnParser.Tests,
dotnet run --project tests/ChessRules.Tests och node tests/web-special-moves.cjs.
