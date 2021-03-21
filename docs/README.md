## Documentation
To get started with learning how to use the API, we suggest that you consult the Wiki pages of this project. There you will find an overview and detailed information about the API. Other sources of API documentation can be found in the docs folder.

NodeJS programs to generate documentation are in the docs folder. Switch to the ./docs folder before executing any node or npm commands. Be sure to install local node modules requiresd for the NodeJS programs by executing the following:

`npm install`

### ReDoc
Build the ReDoc static HTML by executing the following:

`npm run build-redoc`

The result will be an updated html file under the ./docs/redoc folder. This file can be opened
with a browser to view the API documentation.

### Swagger
Use the swagger-ui and express-js to locally serve the documentation using the swagger ui.

`npm run swagger`

The console will inform you when the server is running. Open a browser and navigate to the url indicated. This provides an interactive Swagger UI documentation. It can be used to directly interact with a running instance of an IO Modality Service.
