#!/bin/bash

IMG_NAME="godot-exporter"

build ()
{
    docker build -t ${IMG_NAME} .
    if [ "$?" != 0 ]; then
        exit $?
    fi
}

make ()
{
    docker run --rm=false -v "/home:/home" -it --entrypoint="./export.sh" -w $SRC --security-opt seccomp:unconfined ${IMG_NAME}:latest
}

default ()
{
    docker run --rm=false -v "/home:/home" -it --entrypoint="/bin/zsh" -w $SRC --security-opt seccomp:unconfined ${IMG_NAME}:latest 
    exit $?
}

SRC=`pwd`
build_image=false
compile=false
launch=false
def=true

while getopts "bmd" option; do
    case $option in
        b)
            build_image=true
            def=false
            ;;
        m)
            compile=true
            def=false
            ;;

        *)
            ;;
        :)
            ;;
    esac
done

if [ "$build_image" = true ]; then
    build
fi
if [ "$compile" = true ]; then
    make
fi
if [ "$def" = true ]; then
    default
fi
