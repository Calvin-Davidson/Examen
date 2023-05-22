# 17 April 2023
- **Van: Ingmar van Busschbach**
- **Naar: Edwin, Linx Interactive**
- **Onderwerp: Technische problemen Voice Chat**

Hallo Edwin,

graag wil ik met u overleggen over een technisch probleem die wij tegen zijn gekomen tijdens het implementeren van voice chat in Unity.

Tijdens het implementeren van voice chat kwamen wij drie opties tegen. Handmatige implementatie via een Unity API, De Vivox plugin (Gratis, meer werk, geen garantie dat het goed werkt), of de Dissonance plugin (80 euro, wel garantie dat het goed werkt).

Nu hebben wij via de Unity API voice chat geïmplementeerd, maar is de audio-kwaliteit vrij laag omdat Unity de microfoon niet goed op kan nemen. Unity verandert de opname in een bestand die dan naar de andere speler gestuurd wordt. Maar als er meer audio opgenomen wordt, wordt dat in een nieuw bestand opgeslagen wat het oude bestand vervangt, in plaats van dat het aan de achterkant van het oude bestand bijgevoegd wordt. Daardoor kapt de audio heel abrupt af tijdens het praten, wat een stotterend resultaat oplevert.

Wij kunnen proberen om via de Vivox plugin een nieuwe implementatie op te zetten met de hoop dat het betere audio-kwaliteit levert, of het laten zoals het nu is. Een volledige nieuwe implementatie zal heel veel kostbare tijd kosten, zonder garantie dat de nieuwe implementatie beter zal werken. Zover wij onderzocht hebben, gebruiken namelijk alle Unity games de betaalde Dissonance plugin, die wij natuurlijk zelf niet kunnen gebruiken zonder budget. Verder is er geen documentatie te vinden hoe die plugin gemaakt is en hoe je dat zelf zou kunnen implementeren.

Wat is uw mening hierover? Denkt u dat het het waard zou zijn om een nieuwe implementatie te proberen met Vivox, of het als proof-of-concept maar laten zoals het nu is?

Groeten,
Ingmar van Busschbach,
Product owner team 9.

# 17 April 2023
- **Van: Edwin, Linx Interactive**
- **Naar: Ingmar van Busschbach**
- **Onderwerp: Technische problemen Voice Chat**

Beste Ingmar,

Ik ben niet technisch genoeg om je hierover te kunnen adviseren, en heb daarom Nils - onze Lead Developer - toegevoegd in de CC.
Hopelijk kan hij jullie goed advies geven.

Groeten,
Edwin

# 17 April 2023
- **Van: Nils, Linx Interactive**
- **Naar: Ingmar van Busschbach**
- **Onderwerp: Technische problemen Voice Chat**

Hi Ingmar,

Mijn naam is Nils en ben de lead game developer bij Linx.

Het enige wat ik zo op afstand kan adviseren is jezelf het volgende afvragen:
- Hoe belangrijk is het dat de voice chat in orde is?
- Kan ik op z'n minst verstaan wat er gezegd wordt?
- Is de huidige implementatie goed genoeg om het concept van een school project/prototype te tonen?
- Hoeveel tijd heb is er nog over om een andere implementatie te maken?
- Is het haalbaar om een andere implementatie te doen en weet je zeker dat deze goed genoeg is om het concept van de game te ervaren?

In andere woorden: perfect hoeft het niet te zijn, en als ervaren game devs kunnen we prima door problemen heen kijken om een concept te kunnen beoordelen.

Als ik nog een tip mag geven, houd een "known issues" lijstje bij en laat deze zien aan iedereen die 't speelt. Zo is men op de hoogte van potentiële issues tijdens het spelen.

Succes!

# 17 April 2023
- **Van: Ingmar van Busschbach**
- **Van: Nils, Linx Interactive**
- **Onderwerp: Technische problemen Voice Chat**

Hallo Nils,

voor het project is het natuurlijk erg belangrijk dat de spelers kunnen communiceren. Dit was ook een eis binnen de originele opdracht. Natuurlijk is er altijd een optie voor communicatie buiten de game door middel van bijvoorbeeld Discord. Je kan grotendeels verstaan wat er gezegd wordt, maar de audio is van slechte kwaliteit. In principe hebben we tot de volgende sprint review tijd om dit te implementeren, maar er zijn ook andere taken die nog af moeten. Het is daardoor een beetje lastig te beoordelen hoeveel tijd we daadwerkelijk zouden hebben voor de implementatie.

Ik neig op dit moment zelf naar het laten zoals het nu is en misschien volgende sprint te proberen om de Vivox plugin te implementeren, aangezien voor deze sprint er altijd een fallback optie is om gebruik te maken van Discord waar nodig. De vraag is dan of jullie dat verder prima zouden vinden, aangezien communicatie binnen het spel dan "net aan voldoende" zou zijn voor deze sprint.

Groeten,
Ingmar van Busschbach,
product owner team 9.
