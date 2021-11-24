#!/bin/bash

gdversion="Godot_v3.3.3-stable_mono_x11_64"

# CHECK WHETHER THE VERSION IS MONO

monoversion=""

if [[ $gdversion == *"mono"* ]]; then
    monoversion="mono/"
fi

# DEDUCE THE HEADLESS NAME AND VERSION

gdheadless="${gdversion/"x11"/"linux_headless"}"
gdexecname="${gdheadless/"headless_"/"headless."}"

version=${gdversion%-*}
version=${version#*Godot_v}

# DEDUCE TEMPLATE FILE NAME

gdtemplate="${gdversion/"x11_64"/"export_templates"}"

# DOWNLOAD GODOT

if [ ! -f "/tmp/$gdheadless/$gdexecname" ]; then
    curl https://downloads.tuxfamily.org/godotengine/$version/$monoversion$gdheadless.zip --output /tmp/godot.zip
    unzip -o /tmp/godot.zip -d /tmp
fi

# INSTALL EXPORT TEMPLATE

monopath=""

if [ ! -z "$monoversion" ]; then
    monopath=".mono"
fi

if [ ! -d "$HOME/.local/share/godot/templates/$version.stable$monopath" ]; then
    mkdir -p $HOME/.local/share/godot/templates/$version.stable$monopath
    curl https://downloads.tuxfamily.org/godotengine/$version/$monoversion$gdtemplate.tpz --output /tmp/templates.zip
    unzip -o /tmp/templates.zip -d /tmp
    mv /tmp/templates/* $HOME/.local/share/godot/templates/$version.stable$monopath
fi

# EXPORT PROJECT

mkdir -p exports

/tmp/$gdheadless/$gdexecname --export HTML5 exports/project