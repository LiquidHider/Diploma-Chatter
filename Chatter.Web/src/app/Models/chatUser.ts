import { Contact } from "./contact";

export interface ChatUser
{
    id: number;
    userName: string;
    lastName: string;
    firstName: string;
    patronymic: string;
    universityName: string;
    universityFaculty: string;
    joined: Date;
    lastActive: Date;
    contacts: Contact[];
}