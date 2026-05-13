export const UserRole = {
  None: 0,
  Customer: 1,
  Manager: 2,
  Admin: 3,
} as const;

export const UserStatus = {
  Unknown: 0,
  Active: 1,
  Inactive: 2,
  Suspended: 3,
} as const;

export type UserRoleValue = (typeof UserRole)[keyof typeof UserRole];
export type UserStatusValue = (typeof UserStatus)[keyof typeof UserStatus];

export type DocumentedUserRegistration = {
  email: string;
  username: string;
  password: string;
  name: { firstname: string; lastname: string };
  address: {
    city: string;
    street: string;
    number: number;
    zipcode: string;
    geolocation: { lat: string; long: string };
  };
  phone: string;
  status: string;
  role: string;
};

export interface ApiResponseWithData<T> {
  success: boolean;
  message?: string;
  data?: T;
}
