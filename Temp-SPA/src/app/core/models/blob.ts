export type FileType = 'Image' | 'Document';

export interface BlobDto {
  uri?: string;
  name?: string;
  contentType?: string;
  folderPath?: string;
  fileType?: FileType;
  createdAt?: Date;
  size?: number;
}

export interface BlobResponse {
  status?: string;
  error: boolean;
  errorMessage?: string;
  errorCode?: string;
  blob: BlobDto;
}
