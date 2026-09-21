Abbiamo creato un microservizio MailService a se perché potrebbe oltre alla conferma di registrazione, psw e altro (comunicando
quindi con l'IdentityService) lo possiamo utilizzare anche per mandare mail ad esempio per il reservation service e via dicendo cosi da
non duplicare la conf e logica smtp ogni volta nei vari microservizi.