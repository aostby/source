# Kolibri.Etiketter

Program for å lage og skrive ut etiketter på A4-etikettark, laget for Lyreco 24 per ark (70 x 37 mm)
og Lyreco 8 per ark (105 x 74 mm).
Flere ark kan velges, se **Etikettark** under. Laget i C# WinForms (.NET 10).

## Kom i gang
1. Åpne `Kolibri.Etiketter.sln` i Visual Studio 2022/2026 (med .NET 10 SDK), eller kjør `dotnet run --project Kolibri.Etiketter`.
2. Skriv teksten i feltet under etiketten. Etiketten på skjermen oppdateres mens du skriver.
3. Velg skrift, størrelse, stil og farge med **Velg skrift...**, og velg **Sentrert** eller **Venstrestilt**.
4. Legg til et bilde med **Legg til bilde...**, ved å dra en bildefil inn på etiketten, eller via **Rediger > Lim inn bilde**.
   Bildet legges til venstre og får 1/3 av bredden, og teksten brytes ved siden av. Bredde og side kan endres.
   Nye etiketter får en 5,25"-diskett som standardbilde (tegnet i programmet). Fjern eller bytt den som et vanlig bilde.
   Velg **Plassering: Midten (uten tekst)** for å få bare bildet, sentrert på hele etiketten.
5. Velg etikettark, hvilke posisjoner på arket som skal skrives ut, og hvor mange ark (se under).
6. Velg skriver under **Utskrift > Skriver**, eller la **Windows' standardskriver** stå (se **Skrivere** under).
7. **Forhåndsvis ark...** viser hele A4-arket. **Skriv ut...** åpner Windows' skriverdialog, der du kan velge
   HP Color Laser 150nw, HP LaserJet Pro M402dne, en annen skriver eller **Microsoft Print to PDF** for å kontrollere resultatet først.

## Etikettark
Velg arket under **Utskrift > Etikettark**. Målene er hentet fra produsentenes maler.

| Ark | Etikett | Oppsett | Marg topp / venstre | Mellomrom |
|---|---|---|---|---|
| Lyreco, Avery Zweckform 3474 (standard) | 70 × 37 mm | 3 × 8 = 24 | 0,5 / 0 mm | ingen |
| Lyreco Multipurpose Labels 3 × 7 | 70 × 37 mm | 3 × 7 = 21 | 0 / 0 mm (38 mm tomt nederst) | ingen |
| Avery L7160 | 63,5 × 38,1 mm | 3 × 7 = 21 | 15,15 / 7,21 mm | 2,54 mm i bredden |
| Herma 4360, Herma 4453 | 70 × 36 mm | 3 × 8 = 24 | 4,5 / 0 mm | ingen |
| Lyreco 8 per ark, Avery Zweckform 3427 | 105 × 74 mm | 2 × 4 = 8 | 0,5 / 0 mm | ingen |
| Avery Zweckform 3651 | 52,5 × 29,7 mm | 4 × 10 = 40 | 0 / 0 mm | ingen |
| Avery L7651 | 38,1 × 21,2 mm | 5 × 13 = 65 | 10,7 / 4,75 mm | 2,5 mm i bredden |
| Avery L7780 (runde) | Ø 40 mm | 4 × 6 = 24 | 13,5 / 16 mm | 6 mm begge veier |

- Navnet i listen starter med produktet og sier om etikettene ligger **kant i kant** eller har **marger**, så ark med
  samme antall (f.eks. 21 per ark) ikke forveksles. Under listen står hvor første etikett ligger.
- Etiketten på skjermen, miniatyrarket, utskriften, kalibreringsarket og varselet om skriverens kant følger arket.
- Valget av posisjoner huskes for hvert ark. Første gang et ark velges, er alle etikettene på det valgt.
- På runde etiketter tegnes bilde og tekst i et liggende felt inni sirkelen (stiplet blå linje).
- Arkene kommer fra etikettverktøyet på nyttig.web.app.

## Teksten på etiketten
- Det du ser på skjermen er det du får på papiret. Teksten tegnes som omriss i millimeter, uavhengig av skjerm- og
  skriveroppløsning, så linjeskift og plass blir nøyaktig like på skjermen, i forhåndsvisningen og på utskriften.
  Du trenger ikke forhåndsvise arket for å se om teksten får plass.
- **Krymp skriften automatisk** er på som standard. Får ikke teksten plass, krympes skriften i små steg til den passer,
  og størrelsen som faktisk brukes vises under skriftvalget, f.eks. «krympet til 12,25 pt for å få plass».
- Får teksten likevel ikke plass (eller automatisk krymping er slått av), vises en rød melding under tekstfeltet.
  Et ord som er for bredt for etiketten regnes også som at teksten ikke får plass.
- Teksten midtstilles loddrett på etiketten.

### Skrift per linje
- **Velg skrift...** gir standardskriften for hele teksten.
- Marker én eller flere linjer i tekstfeltet (det holder at markøren står på linjen) og trykk
  **Skrift for markerte linjer...** for å gi dem egen skrifttype, størrelse, stil og farge.
  **Standardskrift** tar dem tilbake til standardskriften. Under knappene står skriften til linjen(e) du står på.
- Linjeskriften følger linjen når du setter inn eller sletter linjer over. Deler du en linje med Enter,
  får begge delene samme skrift; en ny tom linje får skriften fra linjen over.
- Krympes teksten for å få plass, krympes alle linjene like mye, så forholdet mellom størrelsene beholdes.
- Linjeskriftene lagres i `.etikett`-filen.
- Etikettene ligger helt inntil hverandre, så alt innhold holdes 1 mm fra kanten (stiplet blå linje på skjermen).

## Velge posisjoner og antall ark
- Samme etikett skrives på alle valgte posisjoner. Klikk på en rute på miniatyrarket for å slå den av eller på
  (blå = skrives ut, grå = hoppes over), eller klikk og dra for å gi flere ruter samme valg.
- **Velg alle** og **Fjern alle** gjør det raskt. Nyttig når et ark er delvis brukt, for eksempel når 1–6 og 23–24
  allerede er tatt: trykk **Velg alle** og klikk bort de brukte rutene.
- **Antall ark** sier hvor mange ark som skrives ut. Det starter alltid på 1, og musehjulet endrer ikke tallfeltene (bare panelet ruller). Hvert ark får etiketter på nøyaktig de valgte
  posisjonene, og det lages aldri ekstra ark. Under står f.eks. «18 etiketter på 1 ark».
- Er ingen posisjoner valgt, kan du ikke forhåndsvise eller skrive ut.

## Treffe etikettene
- Lyreco-arket: 3 x 70 mm = 210 mm (ingen sidemarg), 8 x 37 mm = 296 mm (0,5 mm topp og bunn).
- **Skriv ut hjelpelinjer** tegner omrisset av hver etikett. Skriv ut på vanlig papir og legg det over et etikettark
  for å sjekke treffet. Treffer skriveren litt skjevt, juster med **Justering mm** (høyre/ned), eller bruk
  **Kalibrer skriver** (se under).

## Skrivere
Du trenger ikke vite hvilken skriver du har: første valg i listen er **Windows' standardskriver**, og kant,
farger og A4 leses da fra skriverens driver, med justering 0 til skriveren er kalibrert. Det samme gjelder
skrivere programmet ikke kjenner. Modellene i tabellen under gir bare et navn og noen råd.

Hver skriver har sin egen justering, som huskes og brukes automatisk når skriveren velges, enten i listen
**Skriver** eller i Windows' skriverdialog. Under listen står det hva programmet vet om skriveren: modell,
farger eller svart-hvitt, skriverens ikke-utskrivbare kant og om den er kalibrert.

| Skriver | Gjenkjennes på | Farger | Råd |
|---|---|---|---|
| HP Color Laser 150nw (150-serien) | «Color Laser 15» eller «150nw» i navnet | ja | papirtype «Etiketter» |
| HP LaserJet Pro M402dne | «M402» i navnet | nei (gråtoner) | papirtype «Etiketter», ikke tosidig |
| Andre skrivere | – | etter driveren | papirtype «Etiketter» om den finnes |

- **Farger:** på fargeskrivere ber programmet om fargeutskrift, så farget tekst og bilder kommer ut i farger.
  Mange fargeskrivere, også HP Color Laser 150nw, skriver ellers i gråtoner.
- **Papirtype:** velg «Etiketter» (Labels) under **Innstillinger/Egenskaper** i utskriftsdialogen, eller én gang
  for alle i Windows (**Innstillinger > Bluetooth og enheter > Skrivere og skannere > skriveren > Utskriftsinnstillinger**).
  Da varmer skriveren mer, så toneren fester seg på etikettene og ikke smitter av.
- **Kant:** de fleste laserskrivere kan ikke skrive helt ut til kanten av arket (HP-ene har rundt 4 mm).
  Havner tekst eller bilde der på noen av de valgte etikettene, vises en rød melding, og programmet spør før utskrift.
  Lyreco-arket har etikettene helt ut til kanten, så på venstre og høyre kolonne må innholdet holdes litt unna kanten.
- Justeringen som var felles for alle skrivere før, flyttes til skriveren som sist ble brukt.

### Skriverdiagnose
**Hjelp > Skriverdiagnose...** viser, uten å skrive ut, hva som sendes til skriverdriveren: etikettark og marger,
papir (A4 stående?), skriverens kant, kopier, tosidig og hvor mange sider programmet lager. Teksten kan kopieres.

- **Skriv ut ... ark** viser hvor mange ark som skrives ut. Knappen **Skriv ut...** i forhåndsvisningen går via den
  samme skriverdialogen og de samme sjekkene (A4, kanten), og lagrer valgene.
- Har driveren ikke A4, advares det før utskrift.

### Kalibrer skriver
**Fil > Kalibrer skriver...** (eller knappen under justeringen):
1. **Skriv ut kalibreringsark** på vanlig A4-papir. Arket skrives ut uten justering og har et kryss med
   millimeterskala 20 mm fra hvert hjørne, omrisset av etikettene, skriverens utskrivbare område (stiplet) og en fargeprøve.
2. Mål med linjal fra arkets venstre og øvre kant til midten av kryss A (øverst til venstre). Begge skal være 20 mm.
   Mål gjerne også kryss B (nederst til høyre) fra høyre og nedre kant, så brukes snittet.
3. Skriv inn målene og trykk **Bruk justeringen**. Programmet regner ut og lagrer justeringen for skriveren.

## Filer
- Etiketter lagres som `.etikett` (JSON med bildet innebygd) via **Fil > Lagre**.
- Siste etikett og innstillinger (skriver, justering per skriver, etikettark, valgte posisjoner per ark, antall ark) huskes i `%AppData%\Kolibri.Etiketter`.
  Innstillinger fra den tidligere mappen `%AppData%\Etikettprogram` kopieres over automatisk første gang.
- **Hjelp > Om** viser denne beskrivelsen inne i programmet.

## Release
`deploy.ps1` bygger én selvstendig exe (win-x64, trenger ikke .NET installert) og kopierer den til
`\\Greenlantern\apps\RELEASE\Kolibri.Etiketter`. Andre filer i mappen røres ikke.
Lukk programmet der før du kjører skriptet. `.\deploy.ps1 -Target .\publish` bygger lokalt i stedet.

## Kode
Alt ligger i namespace `Kolibri.Etiketter`.

| Fil | Innhold |
|---|---|
| `LabelSheet.cs` | Arkene man kan velge, og geometrien deres (mål og posisjoner i mm) |
| `LabelDesign.cs` | Innholdet på etiketten, lagring/lasting |
| `LabelRenderer.cs` | Tegner etiketten i mm (teksten som omriss), brukes både på skjerm og skriver |
| `LabelPreviewControl.cs` | Forstørret visning av én etikett |
| `SheetMapControl.cs` | Miniatyrark for valg av hvilke posisjoner som skrives ut |
| `PrinterDiagnostics.cs` | **Hjelp > Skriverdiagnose** |
| `PrinterProfiles.cs` | Justering per skriver, kjente skrivermodeller, farger og ikke-utskrivbar kant |
| `CalibrationPrintJob.cs` | Kalibreringsarket |
| `CalibrationForm.cs` | **Kalibrer skriver**, regner ut justeringen fra målene |
| `FloppyDiskImage.cs` | Disketten som er standardbilde på nye etiketter, tegnet med GDI+ |
| `LabelPrintJob.cs` | Utskrift, A4 uten marger, kompenserer for skriverens ikke-utskrivbare kant |
| `AppSettings.cs` | Innstillinger som huskes mellom hver gang programmet startes |
| `AboutForm.cs` | **Hjelp > Om**, viser README.md (lagt inn i programmet) |
| `MainForm.cs` | Skjermbildet (bygget i kode, ikke i designer) |
