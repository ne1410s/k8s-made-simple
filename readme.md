# Kubes cheat sheet

## Local Development

### Running Docker Containers
The following may be useful when developing locally:

```bash
# run rabbitmq disposably
 docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management

 # run clamav disposably
 docker run -it --rm --name clamav -p 3310:3310 clamav/clamav:1.2
```

### Building Images
To build yourself a docker image for your app, do the following:
- Create a `Dockerfile` for your app
- Build it, for example by cd'ing to its directory, then:
  - `docker build .`
- Find the id of your newly-built image:
  - `docker images`
- Tag your image ready for pushing to a repo, e.g.
  - `docker tag <IMAGE_ID> <REPO>/<APP_NAME>:<SEMVER>`
- Push the tagged image to the repo:
  - `docker push <REPO>/<APP_NAME>:<SEMVER>`

With the image pushed to a repo, you can now reference it from kubes manifest files (e.g. Pods, Deployments). 

*You will need to ensure the agent/environment has appropriate access to the image repo of course.*

## Cluster Behaviour

### Setting Up OpenLens
OpenLens or similar is highly recommended to visualise your cluster and the resources therein. If going with OpenLens, the latest versions do not allow pod log access, unless the following extension is added:

`@alebcay/openlens-node-pod-menu`

### Updating the Cluster
We can upsert kubes resources using `kubectl` to apply declarative yaml file(s). The following commands come in handy:

```bash
# apply manifest file (or folder of files)
kubectl apply -f <MANIFEST_PATH>

# get everything for a given namespace
kubectl get all -n <NAMESPACE>

# describe deployment (for example)
kubectl describe deployment <DEPLOY_NAME>
```

Commands without the `-n <NAMESPACE>` will apply to the default namespace only, hence will need to be specified where a resource lives in a non-default namespace.

#### Request Size (Ingress resource config)
*Ingress* requests are capped at about 3mb by default.
to change, use following in the Ingress > metadata > annotations block:

`nginx.ingress.kubernetes.io/proxy-body-size: "20m"`


### Accessing Kubes Services
#### Internal Access from other Resources
When a kubes pod-like resource (e.g Pod, Deployment, etc) is exposed internally within the cluster (typically with a ClusterIP service), then those pods are accessible from other resources via:

`http://<SERVICENAME>.<NAMESPACE>.svc.cluster.local`

(This is conventional kubernetes behaviour. Note that other mechanisms may prevent access from succeeding, such as a service mesh).

#### External Access
Exposing a ClusterIP service alone will not make it accessible from outside of the cluster. This is typically done via one of the following ways:
- Specifying a static node port (using a NodePort resource, which is very similar to ClusterIP)
- Specifying a LoadBalancer resource (but these will not work locally, at least not out-of-the-box, without some additional supporting resources and configuration)
- Specifying an `IngressController`...

##### Ingress Controller Approach
An `IngressController` along with specific `Ingress` resources can be used to facilitate inbound traffic external to the cluster, and generically configure how it gets handled.  It is highly recommended:
  - It helps to simplify the application of common access rules since all external traffic to the cluster must route through it
  - It also works locally in the same way as a *hosted* cluster would

##### TLS / SSL Certificates
A common approach is to terminate TLS on ingress to the cluster, meaning that downstream (in-cluster) traffic can be conducted over `http` (as opposed to `https`). This provides the security benefits that certificates bring, without unnecessarily enforcing it in places where it brings little or no benefit, only adding latency.

Having `cert-manager` in your cluster's arsenal is highly recommended for (and sometimes a pre-requisite to) running various other resources.  One of its capabilities is the full end-to-end automation of obtaining and configuring Let's Encrypt SSL certificates for your cluster..

NB: The cluster must be available to acme servers (i.e. hosted and accessible) for this e2e to function correctly.

##### Using ACME Certs in Local Development
One approach is to obtain a wildcard SSL certificate from your favourite CA (i.e. Let's Encrypt!) and add corresponding entries to your hosts file for each exposed Ingress:
1. Obtain a wildcard cert on `*.local.<YOUR_DOMAIN>...`
1. Add a corresponding `Secret` to each namespace in your cluster, that contains the crt and key, as Base64 encoded strings
1. Configure your `Ingress` resources to use the above `Secret` for its given namespace, with the appropriate route, e.g. `yourservice.local.<YOUR_DOMAIN>...`
1. Add a hosts file entry like:
   - `127.0.0.1  yourservice.local.<YOUR_DOMAIN>...`

With the above in place, DNS lookups from your machine to `yourservice.local.<YOUR_DOMAIN>` will get routed to your local cluster, and the IngressController will rightly recognise these as trusted, authentic connections for SSL, based on the certificate and domain matching perfectly that for which it was issued ;)

There's obviously many alternatives to the above, but it does at least involve a legitimate TLS termination, in a manner that is extremely similar to how the cluster will behave within a hosted context.

## Useful Links
Example ingress controller manifest (nginx):
- https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

Example cert manager manifest:
- https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.yaml

Example prometheus manifests:
- https://github.com/techiescamp/kubernetes-prometheus

Prometheus - can then also set up the following:
- kube-state-metrics
- alert-monitor
- grafana

Guidance for each of the above may be found:
- https://devopscube.com/setup-prometheus-monitoring-on-kubernetes/#setting-up-kube-state-metrics

Example open telemetry operator manifest:
- https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml


## TODO

### OpenTel / Prometheus / Loki / Jaeger / Grafana 
### ServiceMesh
### ConfigMaps 
