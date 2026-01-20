#!/bin/sh
printf '\033c\033]0;%s\a' WalterScreenSaver
base_path="$(dirname "$(realpath "$0")")"
"$base_path/walter_screen_saver.x86_64" "$@"
