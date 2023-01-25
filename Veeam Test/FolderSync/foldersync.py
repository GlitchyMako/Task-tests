import os
import shutil
import time
import argparse
import hashlib

def get_md5(file_path):
    """
    Calculate the md5 hash of a file
    """
    hash_md5 = hashlib.md5()
    with open(file_path, "rb") as f:
        for chunk in iter(lambda: f.read(4096), b""):
            hash_md5.update(chunk)
    return hash_md5.hexdigest()

def sync_folders(src, dest, interval, log_file):
    """
    Synchronize the contents of the src folder with the contents of the dest folder
    """
    while True:
        # Get the list of files in the source and destination folders
        src_files = set(os.listdir(src))
        dest_files = set(os.listdir(dest))

        # Calculate the files that need to be added and removed
        files_to_add = src_files - dest_files
        files_to_remove = dest_files - src_files

        # Add new files to the destination folder
        for file_name in files_to_add:
            src_file = os.path.join(src, file_name)
            dest_file = os.path.join(dest, file_name)
            shutil.copy2(src_file, dest_file)
            with open(log_file, "a") as f:
                f.write(f"{time.ctime()}: Added {file_name}\n")
            print(f"{time.ctime()}: Added {file_name}")

        # Remove files from the destination folder that do not exist in the source folder
        for file_name in files_to_remove:
            dest_file = os.path.join(dest, file_name)
            os.remove(dest_file)
            with open(log_file, "a") as f:
                f.write(f"{time.ctime()}: Removed {file_name}\n")
            print(f"{time.ctime()}: Removed {file_name}")

        # update existing files
        for file_name in src_files & dest_files:
            src_file = os.path.join(src, file_name)
            dest_file = os.path.join(dest, file_name)
            if get_md5(src_file) != get_md5(dest_file):
                shutil.copy2(src_file, dest_file)
                with open(log_file, "a") as f:
                    f.write(f"{time.ctime()}: Updated {file_name}\n")
                print(f"{time.ctime()}: Updated {file_name}")

        # Wait for the next synchronization interval
        time.sleep(interval)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("src", help="Source folder path")
    parser.add_argument("dest", help="Replica folder path")
    parser.add_argument("interval", type=int, help="Synchronization interval in seconds")
    parser.add_argument("log_file, help="Path to the log file")
    args = parser.parse_args()
    sync_folders(args.src, args.dest, args.interval, args.log_file)

#create log file if not exists
if not os.path.exists(args.log_file):
    with open(args.log_file,'w'): pass
    
try:
    sync_folders(args.src, args.dest, args.interval, args.log_file)
except KeyboardInterrupt:
    print("Exiting the program")