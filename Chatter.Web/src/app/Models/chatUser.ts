import { Photo } from "./photo";

export interface ChatUser
{
    id: number;
    userName: string;
    lastName: string;
    firstName: string;
    patronymic: string;
    universityName: string;
    universityFaculty: string;
    photos: Photo[];
    mainPhotoUrl: string;
    joined: Date;
    lastActive: Date;
}