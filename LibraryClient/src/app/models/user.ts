export class User {
    id:number;
    name:string;
    surname:string;
    email:string;
    password:string;
    rule:string;

    constructor( id:number, name:string, surname:string, email:string, password:string, rule:string){

        this.id = id;
        this.name = name;
        this.surname = surname;
        this.email = email;
        this.password = password;
        this.rule = rule;
    }
}
