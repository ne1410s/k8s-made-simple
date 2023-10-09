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

### Accessing Services
When a kubes pod-like resource (e.g Pod, Deployment, etc) is exposed internally within the cluster (typically with a ClusterIP service), then those pods are accessible from other nodes via:
`http://SERVICENAME.NAMESPACE.svc.cluster.local`

(This is conventional kubernetes behaviour. Note that other mechanisms may prevent access from succeeding, such as a service mesh).


# upsert everything directly under the manifest folder
kubectl apply -f ./manifest

# check everything in a given namspace
kubectl get all -n fileman


### ingress example
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

### cert-manager example
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.yaml

### prometheus manifests here
https://github.com/techiescamp/kubernetes-prometheus

### otel operator
kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml

### ssl
# fully-automated ssls via certmanager is achievable for HOSTED cluster - as there's a http01 ACME challenge
# when doing the above, see the Ingress resources - where the hard-coded ssl cert secrets currently live :)
## but local dev, we can add host file entries as follows, with a previously issued cert 
127.0.0.1 portal.local.ne1410s.co.uk

### request size
ingress requests capped at about 3mb by default.
to change, use following in the Ingress > metadata > annotations block:
  nginx.ingress.kubernetes.io/proxy-body-size: "20m"

############# TODO

### OpenTel / Prometheus / Loki / Jaeger / Grafana 
### ServiceMesh
### ConfigMaps 
