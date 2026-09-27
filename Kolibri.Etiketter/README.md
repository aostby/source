# Kolibri.Etiketter

Program for å lage og skrive ut Lyreco-etiketter: 24 etiketter per A4-ark, 70 x 37 mm, 3 i bredden og 8 i høyden.
Laget i C# WinForms (.NET 10).

## Kom i gang
1. Åpne `Kolibri.Etiketter.sln` i Visual Studio 2022/2026 (med .NET 10 SDK), eller kjør `dotnet run --project Kolibri.Etiketter`.
2. Skriv teksten i feltet under etiketten. Etiketten på skjermen oppdateres mens du skriver.
3. Velg skrift, størrelse, stil og farge med **Velg skrift...**, og velg **Sentrert** eller **Venstrestilt**.
4. Legg til et bilde med **Legg til bilde...**, ved å dra en bildefil inn på etiketten, eller via **Rediger > Lim inn bilde**.
   Bildet legges til venstre og får 1/3 av bredden, og teksten brytes ved siden av. Bredde og side kan endres.
5. Velg hvilke posisjoner på arket som skal skrives ut, og hvor mange ark (se under).
6. **Forhåndsvis ark...** viser hele A4-arket. **Skriv ut...** åpner Windows' skriverdialog, der du kan velge
   HP LaserJet Pro M402dne, en annen skriver eller **Microsoft Print to PDF** for å kontrollere resultatet først.

## Teksten på etiketten
- Det du ser på skjermen er det du får på papiret. Teksten tegnes som omriss i millimeter, uavhengig av skjerm- og
  skriveroppløsning, så linjeskift og plass blir nøyaktig like på skjermen, i forhåndsvisningen og på utskriften.
  Du trenger ikke forhåndsvise arket for å se om teksten får plass.
- **Krymp skriften automatisk** er på som standard. Får ikke teksten plass, krympes skriften i små steg til den passer,
  og størrelsen som faktisk brukes vises under skriftvalget, f.eks. «krympet til 12,25 pt for å få plass».
- Får teksten likevel ikke plass (eller automatisk krymping er slått av), vises en rød melding under tekstfeltet.
  Et ord som er for bredt for etiketten regnes også som at teksten ikke får plass.
- Teksten midtstilles loddrett på etiketten.
- Etikettene ligger helt inntil hverandre, så alt innhold holdes 1 mm fra kanten (stiplet blå linje på skjermen).

## Velge posisjoner og antall ark
- Samme etikett skrives på alle valgte posisjoner. Klikk på en rute på miniatyrarket for å slå den av eller på
  (blå = skrives ut, grå = hoppes over), eller klikk og dra for å gi flere ruter samme valg.
- **Velg alle** og **Fjern alle** gjør det raskt. Nyttig når et ark er delvis brukt, for eksempel når 1–6 og 23–24
  allerede er tatt: trykk **Velg alle** og klikk bort de brukte rutene.
- **Antall ark** (standard 1) sier hvor mange ark som skrives ut. Hvert ark får etiketter på nøyaktig de valgte
  posisjonene, og det lages aldri ekstra ark. Under står f.eks. «18 etiketter på 1 ark».
- Er ingen posisjoner valgt, kan du ikke forhåndsvise eller skrive ut.

## Treffe etikettene
- Ark: 3 x 70 mm = 210 mm (ingen sidemarg), 8 x 37 mm = 296 mm (0,5 mm topp og bunn).
- **Skriv ut hjelpelinjer** tegner omrisset av hver etikett. Skriv ut på vanlig papir og legg det over et etikettark
  for å sjekke treffet. Treffer skriveren litt skjevt, juster med **Justering mm** (høyre/ned).

## Filer
- Etiketter lagres som `.etikett` (JSON med bildet innebygd) via **Fil > Lagre**.
- Siste etikett og innstillinger (skriver, justering, valgte posisjoner, antall ark) huskes i `%AppData%\Kolibri.Etiketter`.
  Innstillinger fra den tidligere mappen `%AppData%\Etikettprogram` kopieres over automatisk første gang.
- **Hjelp > Om** viser denne beskrivelsen inne i programmet.

## Kode
Alt ligger i namespace `Kolibri.Etiketter`.

| Fil | Innhold |
|---|---|
| `LabelSheet.cs` | Arkets geometri (mål og posisjoner i mm) |
| `LabelDesign.cs` | Innholdet på etiketten, lagring/lasting |
| `LabelRenderer.cs` | Tegner etiketten i mm (teksten som omriss), brukes både på skjerm og skriver |
| `LabelPreviewControl.cs` | Forstørret visning av én etikett |
| `SheetMapControl.cs` | Miniatyrark for valg av hvilke posisjoner som skrives ut |
| `LabelPrintJob.cs` | Utskrift, A4 uten marger, kompenserer for skriverens ikke-utskrivbare kant |
| `AppSettings.cs` | Innstillinger som huskes mellom hver gang programmet startes |
| `AboutForm.cs` | **Hjelp > Om**, viser README.md (lagt inn i programmet) |
| `MainForm.cs` | Skjermbildet (bygget i kode, ikke i designer) |
