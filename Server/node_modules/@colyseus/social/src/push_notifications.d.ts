import PushNotifications from 'node-pushnotifications';
export declare function sendNotification(data: PushNotifications.Data, registrationIds?: PushNotifications.RegistrationId | PushNotifications.RegistrationId[]): Promise<{
    success: number;
    failure: number;
}>;
export declare const ServiceWorkerScript: string;
