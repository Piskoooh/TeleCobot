#!/usr/bin/env python3

import open3d as o3d
import os
import sys

###remove_statistical_outlierによるノイズ除去
## 実行前に　chmod +x remove_statistical_outlier.py 　を実行しておくこと
# 以下で実行
# python3 remove_statistical_outlier.py input_file_path output_file_path


def main(file_path, output_file_path=None):
    # 点群データの読み込み
    pcd = o3d.io.read_point_cloud(file_path)

    # ダウンサンプリングを行う
    downpcd = pcd.voxel_down_sample(voxel_size=0.05)  # voxel_sizeを調整して適切なサンプリングサイズを設定

    # ノイズ除去を行う
    cl, ind = downpcd.remove_statistical_outlier(nb_neighbors=20, std_ratio=2.0)  # パラメータは適宜調整可能

    if output_file_path is None:
        # 出力ファイルパスの生成
        file_dir, file_name = os.path.split(file_path)
        output_file_path = os.path.join(file_dir, f"{os.path.splitext(file_name)[0]}_denoise{os.path.splitext(file_name)[1]}")

    # 結果の保存
    o3d.io.write_point_cloud(output_file_path, cl)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python script_name.py input_file_path [output_file_path]")
        sys.exit(1)
    
    input_file_path = sys.argv[1]
    if len(sys.argv) > 2:
        output_file_path = sys.argv[2]
    else:
        output_file_path = None

    main(input_file_path, output_file_path)

