export interface User {
    id:number;
    name:string;
    surname:string;
    email:string;
    role:number;
    phoneNumber: string | null;
    profileImageUrl: string | null;
    createdAt: string | null;    
}
