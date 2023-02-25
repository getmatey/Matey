<p align="center"><img src="./matey-icon-round.png" alt="Matey"/></p>

[![GitHub](https://img.shields.io/github/license/getmatey/Matey)](https://github.com/getmatey/Matey/blob/master/LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)
[![GitHub issues](https://img.shields.io/github/issues/getmatey/Matey)](https://github.com/getmatey/Matey/issues)
[![GitHub Workflow Status (branch)](https://img.shields.io/github/actions/workflow/status/getmatey/Matey/msbuild.yml?branch=master)](https://github.com/getmatey/Matey/actions/workflows/msbuild.yml)
[![GitHub Repo stars](https://img.shields.io/github/stars/getmatey/Matey?style=social)](https://github.com/getmatey/Matey/stargazers)

Matey is a container ingress configurator for traditional web servers. It uses container system events to set up reverse proxies and load balancers for your containers. Instead of reinventing the web server, we leverage tried and tested software.

Currently, Matey supports Docker and IIS. We chose this combination as our starting point because there are working solutions for Linux, such as Traefik. There are no such solutions for Windows hosts.

## Quick Start
With this guide, you will install Matey and configure it to work alongside IIS and Docker.

### Prerequisites
- Docker ([Installation guide](https://learn.microsoft.com/en-us/virtualization/windowscontainers/quick-start/set-up-environment?tabs=dockerce))
- IIS 7.0+ ([Installation guide](https://learn.microsoft.com/en-us/iis/web-hosting/web-server-for-shared-hosting/installing-the-web-server-role))
- ARR module for IIS ([Installation guide](https://learn.microsoft.com/en-us/iis/extensions/installing-application-request-routing-arr/install-application-request-routing))

### Installation
Matey is published as a Microsoft Software Installer (.msi) which can be found on the ["Releases" page](https://github.com/getmatey/Matey/releases/latest). To install, download the .msi and run the installer on your server.

Once the installer is finished, you should see "Matey Configurator Service" in the "Services" list. The service is now running and waiting for container events!

### Start a Container
With everything installed, you can now set up a container which IIS will forward to. Run the following command to try it:
```
docker start --label matey.enabled=true --label matey.frontend.rule=Host:hello-world.localhost getmatey/hello-world
```

This will start our example `hello-world` container and make it accessible through `hello-world.localhost` on your server.
There is also an equivalent [docker-compose.yml](./docker-compose.yml), if you prefer.

Having run this, head to [http://hello-world.localhost](http://hello-world.localhost) in a browser on your server. You should be greeted with a "Hello, world!"

Welcome aboard! You've now run your first container with Matey. If you're looking for more advanced functionality, please head to the [configuration guide](https://getmatey.github.io/).

## Credits
Thanks to [Becca Alizzi](https://alizziillustration.com/) for her beautiful work on the whale and sail boat logo and documentation illustrations.

All Matey logos and illustrations are licensed under the Creative Commons 3.0 Attributions license.