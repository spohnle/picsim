                    00001           ;Programm zum Test des 16F84-Simulators.
                    00002           ;Es werden alle wichtigen Befehle überprüft.
                    00003           ;(c) St. Lehmann   Ersterstellung: 23.01.2003
                    00004           ;14.01.2004 TRIS und OPTION ersetzt durch MOVWF
                    00005           ;
                    00006           ;Definition einiger Symbole
                    00007           indirect equ 0
                    00008           status   equ 03h
                    00009           fsr      equ 04h
                    00010           ra       equ 05h
                    00011           rb       equ 06h
                    00012           count    equ 0ch
                    00013           
                    00014           ;Definition des Prozessors
                    00015           device 16F84
                    00016           ;Festlegen des Codebeginns
                    00017           org 0
                    00018  start    
0000 2817           00019           goto main           ;Unterprogramme überspringen
                    00020           ;****** Hier liegen die gesamten Unterprogramme
                    00021           ;der Speicherbereich 10h bis 1fh wird mit 00h bis 0fh gefül
                    00022  fillinc  
0001 3010           00023           movlw 16            ;Schleifenzähler
0002 008C           00024           movwf count
0003 3010           00025           movlw 10h           ;Startzeiger initialisieren
0004 0084           00026           movwf fsr           ;Zeiger ins FSR
0005 0100           00027           clrw
                    00028  loop1    
0006 0080           00029           movwf indirect      ;Wert indirekt abspeichern
0007 0A84           00030           incf fsr            ;Zeiger erhöhen
0008 3E01           00031           addlw 1             ;W-Register erhöhen (es gibt kein INC W
0009 0B8C           00032           decfsz count        ;Schleifenzähler erniedrigen
000A 2806           00033           goto loop1          ;wiederholen bis F08 auf 0 ist
000B 3400           00034           retlw 0
                    00035           ;Es wird die Summe aus den Inhalten von 10H bis 1Fh gebilde
                    00036           ;(Quersumme, wird oft als einfache Prüfsumme verwendet werd
                    00037  qsumme   
000C 3010           00038           movlw 10h           ;Schleifenzähler initialisieren
000D 008C           00039           movwf count
000E 0084           00040           movwf fsr           ;Startzeiger initialsieren
000F 0100           00041           clrw                ;Summenregister löschen
                    00042  loop2    
0010 0700           00043           addwf indirect,w    ;Speicherinhalt zu W addieren
0011 0A84           00044           incf fsr
0012 0B8C           00045           decfsz count
0013 2810           00046           goto loop2
0014 008F           00047           movwf 0fh           ;Ergebnis abspeichern
0015 098F           00048           comf 0fh            ;Komplement bilden
0016 3400           00049           retlw 0             ;Unterprogrammende
                    00050           ;****** Hier beginnt das Hauptprogramm  **************     
                    00051  main       
0017 303F           00052           movlw 3fh           ;zuerst den Vorteiler vom RTCC trennen
0018 1683           00053           bsf status,5        ;ins Option-Register schreiben
0019 0081           00054           movwf 1             ;=freg 81h
001A 1283           00055           bcf status,5        ;zurück auf Bank0
001B 0100           00056           clrw                ;RTCC-Register löschen
001C 0081           00057           movwf 1h
001D 2001           00058           call fillinc        ;Speicherbereich füllen
001E 200C           00059           call qsumme         ;Quersumme berechnen
001F 090F           00060           comf 0fh,w          ;Ergebnis holen
0020 020F           00061           subwf 0fh,w         ;vom Orginalwert abziehen
0021 008E           00062           movwf 0eh           ;neues Ergebnis abspeichern.
0022 3010           00063           movlw 10h             
0023 1683           00064           bsf status,5        ;auf Bank 1 umschalten
0024 0085           00065           movwf 5             ;=freg 85H  Port A 0-3 auf Ausgang
0025 1283           00066           bcf status,5        ;zurück auf Bank 0
0026 0085           00067           movwf ra            ;Signale auf Low
                    00068  main1    
0027 1806           00069           btfsc rb,0
0028 2827           00070           goto main1          ;warten bis RB0 auf 0
                    00071  main2    
0029 1C06           00072           btfss rb,0
002A 2829           00073           goto main2          ;warten bis rb0 wieder 1         
002B 3020           00074           movlw 20h           ;Option neu setzen, VT=1:2
002C 1683           00075           bsf status,5        ;Bank 1
002D 0081           00076           movwf 1             ;hier liegt Option
002E 1283           00077           bcf status,5        ;wieder Bank 0
                    00078           ;beim Anklicken von RA4 muss bei steigender Flanke der RTCC
                    00079           ;hochzählen (jede 2. Flanke wird gezählt)        
                    00080  ende     
002F 282F           00081           goto ende           ;Endlosschleife, verhindert Nirwana
                    00082             
                    00083             
