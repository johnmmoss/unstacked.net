
export interface AppConfiguration {
  apiUrl:string; 
}

const defaultConfiguration: AppConfiguration = {
  apiUrl: process.env.REACT_APP_API_URL || "http://localhost:5234",
};

export const getConfig = (): AppConfiguration => {
  return defaultConfiguration;
};

export const getApiUrl = (path: string): string => {
  const { apiUrl } = getConfig();
  return `${apiUrl}/${path}`;
};