# Sneky
Snake reeducation game for the [MyoCoach](https://orthopus.com/en/myocoach/)

# Installation

Disclaimer ! These installation steps are for linux users.

## Export Without Docker

Note: if you want to export without docker, you need to install dotnet-sdk and the export templates yourself.
If you have every dependencies installed, you can export the project via the `export.sh` script.
Simply run:
```
./export.sh
```
This will build the project and export it in the `exports/` folder.

## Export With Docker

We provide a docker image to help you flawlessly export the game.
To export the project via the docker image, run:
```
./exporter.sh -b
```
This will build the image (this step needs to be done only once).
Then run this to export the project:
```
./exporter.sh -m
```
This will launch the container and give it `export.sh` as an entrypoint.

If for some reason you need to launch the container in an interactive way, you can run:
```
./exporter.sh
```
