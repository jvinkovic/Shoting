U datoteci config.txt se nalaze postavke. 
Prvo je timetorun što oznaèava koliko duga da aplikacija radi.
Drugo je intervalInSeconds i to oznaèava svakih koliko sekundi da se napravi screenshot.
Treæe je destinationMail no koji da se šalju screenshotovi.

U sluèaju da se želi promijenit na 9 minuta trajanje i screenshot svakih 40 sekundi,
te mail na mojmail@mail.com tada to izgleda ovako:
timetorun=30
intervalInSeconds=12
destinationMail=mojmail@mail.com

NAPOMENA!!!!

Iza brojeva i maila NEMA razmaka ili bilo kakvih dodatnih znakova. 
Kao što nema ni poslije = razmaka.
Ako ih bude, neæe se uzimati u obzir i program æe raditi predefiniranih 15 minuta
s intervalom za screendshot od 7 sekundi.
