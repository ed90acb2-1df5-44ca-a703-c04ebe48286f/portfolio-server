docker run --rm \
  -v $(pwd)/docs:/out \
  -v $(pwd)/src/Portfolio.Protocol/src/Proto:/protos \
  pseudomuto/protoc-gen-doc --doc_opt=markdown,protocol.md