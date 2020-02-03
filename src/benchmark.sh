#!/bin/sh -e

base="$(dirname "$0")"

# Check for required binaries in $PATH
for binary in dotnet jq; do
	if ! which "$binary" > /dev/null; then
		echo >&2 "error: no '$binary' executable in path"
		exit 1
	fi
done

# Run benchmark
dotnet run -c Release -f netcoreapp3.1 -p "$base"/Cottle.Benchmark -- --disableLogFile --exporters JSON --filter CompareBenchmark

# Transform and output JSON result
echo -n 'var benchmarks = '

< BenchmarkDotNet.Artifacts/results/Cottle.Benchmark.Benchmarks.CompareBenchmark-report-full-compressed.json jq -cj '
.Benchmarks |
map([(.Parameters | sub("Engine="; "")), (.Method | ascii_downcase), (.Statistics.Mean | round)]) |
reduce .[] as $item ({}; . * {($item[0]): {($item[1]): $item[2]}})'

echo ';'
