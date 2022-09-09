Dit is een korte handleiding om MCA-light te draaien.

Benodigdheden:
==============
- een RUN.bat bestand:
	+ met een MCA-executable met de volgende argumenten
		+ InvesteringsPlannen, opgeslagen als cases in MCA-bestand
		+ GridOpdracht: Investeringsplan + Scenario
		+ Plek Scenario's (ETM-bestanden)
		+ Plek waar de outputbestanden moeten komen te staan.
	+ formaat zie vb. 'run.bat'
- een MCA-bestand:
	+ met alle topologie, incl. shippers en balansbronnen 
	+ eigenschappen van het ijzerwerk (in volumieke flow)
	+ prognoses van randelementen die niet uit ETM komen (in MW, hv=basise=36)
	+ GEEN rekenopdracht.
- een GRIDopdracht-bestand (in CSV-formaat):
	+ met alle combi's van investeringsplannen met uur-scenario's 
		+ investeringsplannen zijn de cases uit een MCA-bestand. Niet de tijdreeksen
		+ uur-scenario's zijn de ETM-bestanden. 
	+ formaat zie vb. 'Rekenopdracht.csv'
- 'External prognose' bestanden:
	+ uur-prognoses van bijv. de ETM-randelementen in MW
	+ formaat zie vb. 'ETM_2021.csv'

Werking:
========
De combi's van investeringsplannen + uurscenario's (regel uit GRIDopdracht-bestand) zullen als een tijdreeks
alleen DRUKLOOS, isotherm en iso-kwal worden doorgerekend. 
De tijdreeks heeft dan de naam: <Naam investeringsplan> + '_' + <naam  'External prognose' bestand>.
Er zullen 8760 uren worden doorgerekend.

Hierbij wordt alleen gebruikt van:
- temp.onafhankelijke shipperprognoses [MW]
- volumieke capaciteiten van ijzer-elementen [dam3/h]
- preferentie-kosten ev. wensflow shippers 
- transporttarieven -en boetes leidingen en mengreduceers.

Er is GEEN console (!!!) en qua output zal het volgende ALTIJD worden weggeschreven naar 
de OPGEGEVEN OUTPUT-DIRECTORY in 'run.bat':
- algemeen.txt
- plp.txt , indien er iets te melden valt.
- omo-bestand, met GEEN oorzaken ! Eenheid is MW, tenzij anders opgegeven met testoptie.

Afhankelijk van testopties kunnen meerdere outputbestanden weggeschreven worden (zie extra's).

Indien geen output-directory is opgegeven dan wordt de output weggeschreven naar '<naam mca-bestand>.MCT'
Indien in het MCA-bestand een REKENOPDRACHT staat dan wordt de output ook weggeschreven naar '<naam mca-bestand>.MCT'.

In de tijdreeks zullen ALLEEN de tijdstippen uit het 'External prognose' bestand doorgerekend worden.
Bij het rekenen zullen de 'External prognose' randelementen de prognoses aannemen uit het 'External prognose' bestand voor het gedefinieerde tijdstip.
De NIET-'External prognose'-randelementen zullen hun MCA-prognoses aannemen. (denk bv. aan balansbronnen).

Er zullen allerlei controles uitgevoerd. 
o.a. aanwezigheid bestanden en bestaan 'External prognose' randelementen als MCA-shipper-element.

Returncodes:
============
   RC_OverDate                = 10 ;   {Programma is over de houdbaarheidsdatum}
   RC_No_RunArgumenten        = 20 ;   {Er zijn geen argumenten opgegeven voor het programma}
   RC_No_RootDirectory        = 30 ;   {Root voor een output-directory bestaat niet}
   RC_No_OUTPUT_RootDirectory = 33 ;   {Root output-directory bestaat niet}
   RC_No_INPUT_RootDirectory  = 35 ;   {Root MCA-file bestaat niet}
   RC_No_MCAbestand           = 40 ;   {MCA-input-bestand bestaat niet}
   RC_Overflow_name_table     = 43 ;   {Teveel items; MCA-input-bestand is te groot}
   RC_DataFout_MCAbestand     = 45 ;   {MCA-input-bestand heeft parser fouten}
   RC_NO_GridOpdrachtBestand  = 50 ;   {GRID-opdracht-bestand bestaat niet}
   RC_DataFout_GRIDopdracht   = 55 ;   {GRID-opdracht-bestand heeft parser fouten}
   RC_NO_ETMbestand           = 60 ;   {ETM-bestand bestaat niet}
   RC_DataFout_ETMbestand     = 65 ;   {ETM-bestand heeft parser fouten}

Extra's (testopties):
=====================
In het MCA-bestand kunnen TESTOPTIES opgegeven worden, zijnde:

tbv het rekenen:
'b','B': er wordt met cold-restart gerekend.
'k','K': in de frequentie-file geldt de decimale komma (default is punt).
'x','X': Er wordt alleen aanbod -en afzet gerekend, GEEN netwerk-berekening.
'~'    : ruimere tolerantie.

tbv de output:
'c','C': alle uurcases worden als output weggeschreven.
'f','F': alle faalcases worden als output weggeschreven.
'g','G': Alle bestanden voor de GTI (incl. grafische) worden aangemaakt (In gebruikelijke (produktie) eenheden)
'p','P': PUN-file wordt weggeschreven. Flow is in MW
'e','E': ELE-file wordt weggeschreven. Flow is in MW
'y','Y': ijzer-elementen worden in TelQuel weggeschreven in de OMO-file. 
't','T': TIJD-file wordt weggeschreven.
'z','Z': FREQ-files (txt,csv) worden weggeschreven.

vor de modelleerder zijn nog twee testopties aanwezig:
'm','M': Het plp-probleem is uitgeschreven in plp.txt
'v','V': Alle variabele worden getoond in algemeen.txt

Randvoorwaarden
==============-
De executable die gebruikt is, is een MCA-light versie.
Alleen het essentiele is gebruikt !!!	(naspelen met produktie: gebruik testoptie '!' ; rekenoptie hv = basise= 36 of 35.17)

Kies voor de waterstof-bronnen (entry, berging): hv =29.7  !!!!!

Het MCA-bestand kan ingelezen worden met produktie of met een beta, die ook met de 'External prognose' bestanden kan omgaan.
Hier kan dan bv. met druk gerekend worden.

Om met druk te rekenen, zijnde de leiding-eigenschappen LENGTE, DIAMETER en RUWHEID toegestaan.
Als ook DRUKSTURINGEN op knopen en WERKGEBIEDEN van de compressor en de (meng)reduceer.
Verder kunnen ook SPELDEN ingelezen worden. 
In MCA-light wordt hier NIETS mee gedaan.
