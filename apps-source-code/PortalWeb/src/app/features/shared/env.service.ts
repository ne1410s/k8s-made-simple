export class EnvService {
  /** The root url of the api. */
  apiUrl: string;

  constructor(obj: any) {
    this.apiUrl = obj.apiUrl;
  }
}

export const EnvServiceProvider = {
  provide: EnvService,
  useFactory: () => new EnvService((window as any).__env),
};
